using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Emails;
using ClassManager.Infrastructure.BackgroundTasks;
using ClassManager.Infrastructure.Email;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Infrastructure.WebPush;
using ClassManager.Notifications.WebPush;

namespace ClassManager.Infrastructure.Notifications;

internal sealed class OrderNotificationService(
    AppDbContext context,
    IIdentityService identityService,
    IBackgroundTaskOutbox backgroundTasks,
    IWebAppLinks webAppLinks,
    PushPublisher pushPublisher,
    TeamNotifier teamNotifier)
    : IOrderNotificationService
{
    public async Task OrderPlacedAsync(Order order, CancellationToken cancellationToken)
    {
        var business = await CurrentBusinessAsync(cancellationToken);
        var client = await ClientOfAsync(order, cancellationToken);
        if (business is null || client is null)
        {
            return;
        }

        var total = OrderEmails.Amount(order.Total, order.CurrencyCode);
        var deliveryClassName = order.HasProducts ? await DeliveryClassNameAsync(order, cancellationToken) : null;
        var staffUserIds = await StaffUserIdsAsync(business, cancellationToken);
        foreach (var recipient in await RecipientsOfAsync(staffUserIds, cancellationToken))
        {
            var delivery = order.HasProducts ? recipient.Texts.OrderDeliveryForBranch(deliveryClassName) : null;
            var placed = recipient.Texts.OrderPlaced(DetailsOf(order, business), client.FullName, delivery, webAppLinks.TeamOrders());
            await SendAsync(OrderEmails.To(recipient, business.Id, placed), cancellationToken);
        }

        await teamNotifier.NotifyAsync(
            staffUserIds,
            new PushMessage(
                TeamNotificationTexts.OrderPlacedTitle(EmailTexts.Spanish.OrderNumber(order.Number), client.FullName),
                total,
                TeamNotificationTexts.OrdersUrl),
            cancellationToken);
    }

    public async Task OrderPaidAsync(Order order, CancellationToken cancellationToken)
    {
        if (order.Channel != OrderChannel.App)
        {
            return;
        }

        var business = await CurrentBusinessAsync(cancellationToken);
        if (business is null)
        {
            return;
        }

        var deliveryClassName = order.HasProducts && order.IsReady ? await DeliveryClassNameAsync(order, cancellationToken) : null;
        if (order.HasProducts && order.IsReady)
        {
            await PushReadyAsync(order, WhereToGetIt(EmailTexts.Spanish, business, deliveryClassName), cancellationToken);
        }

        foreach (var recipient in await ClientRecipientsAsync(order, cancellationToken))
        {
            var nextStep = !order.HasProducts ? recipient.Texts.OrderClassesCredited
                : order.IsReady ? WhereToGetIt(recipient.Texts, business, deliveryClassName)
                : recipient.Texts.OrderWillNotifyWhenReady;
            var paid = recipient.Texts.OrderPaid(DetailsOf(order, business), nextStep, order.HasProducts, webAppLinks.StudentAppOrders());
            await SendAsync(OrderEmails.To(recipient, business.Id, paid), cancellationToken);
        }
    }

    public async Task OrderReadyAsync(Order order, CancellationToken cancellationToken)
    {
        var business = await CurrentBusinessAsync(cancellationToken);
        if (business is null)
        {
            return;
        }

        var deliveryClassName = await DeliveryClassNameAsync(order, cancellationToken);
        await PushReadyAsync(order, WhereToGetIt(EmailTexts.Spanish, business, deliveryClassName), cancellationToken);
        foreach (var recipient in await ClientRecipientsAsync(order, cancellationToken))
        {
            var ready = recipient.Texts.OrderReady(
                DetailsOf(order, business), WhereToGetIt(recipient.Texts, business, deliveryClassName), webAppLinks.StudentAppOrders());
            await SendAsync(OrderEmails.To(recipient, business.Id, ready), cancellationToken);
        }
    }

    public async Task OrderCancelledAsync(Order order, OrderCancellationReason reason, CancellationToken cancellationToken)
    {
        var business = await CurrentBusinessAsync(cancellationToken);
        if (business is null)
        {
            return;
        }

        foreach (var recipient in await ClientRecipientsAsync(order, cancellationToken))
        {
            var cancelled = reason == OrderCancellationReason.Unpaid
                ? recipient.Texts.OrderCancelledUnpaid(DetailsOf(order, business), webAppLinks.StudentAppOrders())
                : recipient.Texts.OrderCancelledByBranch(DetailsOf(order, business), webAppLinks.StudentAppOrders());
            await SendAsync(OrderEmails.To(recipient, business.Id, cancelled), cancellationToken);
        }
    }

    private static OrderEmailDetails DetailsOf(Order order, Business business) =>
        new(
            order.Number,
            business.BrandDisplayName,
            [.. order.Lines.Select(line => OrderEmails.Line(line.Quantity, line.Name, line.Total, order.CurrencyCode))],
            OrderEmails.Amount(order.Total, order.CurrencyCode));

    private async Task PushReadyAsync(Order order, string whereToGetIt, CancellationToken cancellationToken)
    {
        if (order.ClientId is { } clientId)
        {
            await pushPublisher.PublishToStudentsAsync(
                [clientId], new PushMessage(StudentAppPushTexts.OrderReadyTitle, whereToGetIt, StudentAppPushTexts.OrdersUrl), cancellationToken);
        }
    }

    private Task<Business?> CurrentBusinessAsync(CancellationToken cancellationToken) =>
        context.Businesses.AsNoTracking().FirstOrDefaultAsync(business => business.Id == context.CurrentTenantId, cancellationToken);

    private async Task<Client?> ClientOfAsync(Order order, CancellationToken cancellationToken) =>
        order.ClientId is { } clientId
            ? await context.Clients.AsNoTracking().FirstOrDefaultAsync(client => client.Id == clientId, cancellationToken)
            : null;

    private async Task<string?> DeliveryClassNameAsync(Order order, CancellationToken cancellationToken) =>
        order.Delivery == DeliveryMethod.InClass && order.DeliveryClassGroupId is { } classGroupId
            ? await context.ClassGroups.AsNoTracking()
                .Where(classGroup => classGroup.Id == classGroupId)
                .Select(classGroup => classGroup.Name)
                .FirstOrDefaultAsync(cancellationToken)
            : null;

    private static string WhereToGetIt(IEmailTexts texts, Business business, string? deliveryClassName) =>
        deliveryClassName is null ? texts.OrderPickupAt(business.Name) : texts.OrderInClass(deliveryClassName);

    private async Task<IReadOnlyList<OrderEmailRecipient>> ClientRecipientsAsync(Order order, CancellationToken cancellationToken)
    {
        if (order.ClientId is not { } clientId)
        {
            return [];
        }

        var userIds = await context.ClientAccounts.AsNoTracking()
            .Where(account => account.ClientId == clientId)
            .Select(account => account.UserId)
            .ToListAsync(cancellationToken);
        if (userIds.Count > 0)
        {
            return await RecipientsOfAsync(userIds, cancellationToken);
        }

        var clientEmail = (await ClientOfAsync(order, cancellationToken))?.Email;
        return clientEmail is null ? [] : [new OrderEmailRecipient(clientEmail, EmailTexts.Spanish)];
    }

    private async Task<IReadOnlyCollection<Guid>> StaffUserIdsAsync(Business business, CancellationToken cancellationToken)
    {
        var members = await context.BusinessMembers.AsNoTracking().ToListAsync(cancellationToken);
        var customRoles = await context.CustomRoles.AsNoTracking().ToDictionaryAsync(role => role.Id, cancellationToken);
        var userIds = members
            .Where(member => PermissionsOf(member, customRoles).Contains(Permissions.Orders.Manage))
            .Select(member => member.UserId)
            .ToHashSet();
        userIds.UnionWith(await context.OrganizationMembers.AsNoTracking()
            .Where(member => member.OrganizationId == business.OrganizationId && member.Role == OrganizationRole.BrandOwner)
            .Select(member => member.UserId)
            .ToListAsync(cancellationToken));
        return userIds;
    }

    private async Task<IReadOnlyList<OrderEmailRecipient>> RecipientsOfAsync(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken) =>
        userIds.Count == 0
            ? []
            : [
                .. (await identityService.ListAccountsAsync(userIds, cancellationToken))
                    .DistinctBy(account => account.Email)
                    .Select(account => new OrderEmailRecipient(account.Email, EmailTexts.For(account.PreferredLanguage))),
            ];

    private static IReadOnlySet<string> PermissionsOf(BusinessMember member, Dictionary<Guid, CustomRole> customRoles) =>
        member.CustomRoleId is { } customRoleId
            ? customRoles.TryGetValue(customRoleId, out var customRole) ? MemberRole.Custom(customRole).Permissions : new HashSet<string>()
            : SystemRolePermissions.Of(member.Role);

    private Task SendAsync(EmailMessage message, CancellationToken cancellationToken) =>
        backgroundTasks.EnqueueAsync(new SendEmailBackgroundTaskCommand(message), cancellationToken);
}
