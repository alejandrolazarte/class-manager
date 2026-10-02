using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Infrastructure.WebPush;
using ClassManager.Notifications.WebPush;
using Microsoft.Extensions.Logging;

namespace ClassManager.Infrastructure.Notifications;

internal sealed partial class OrderNotificationService(
    AppDbContext context,
    IIdentityService identityService,
    IEmailSender emailSender,
    IWebAppLinks webAppLinks,
    PushPublisher pushPublisher,
    TeamNotifier teamNotifier,
    ILogger<OrderNotificationService> logger)
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

        var total = OrderEmails.Amount(order.Total, business.CurrencyCode);
        var delivery = order.HasProducts ? OrderEmails.DeliveryForBranch(await DeliveryClassNameAsync(order, cancellationToken)) : null;
        var staffUserIds = await StaffUserIdsAsync(business, cancellationToken);
        foreach (var email in await EmailsOfAsync(staffUserIds, cancellationToken))
        {
            await SendAsync(OrderEmails.Placed(EmailContextOf(order, business, email), client.FullName, delivery, webAppLinks.TeamOrders()), cancellationToken);
        }

        await teamNotifier.NotifyAsync(
            staffUserIds,
            new PushMessage(TeamNotificationTexts.OrderPlacedTitle(OrderEmails.NumberOf(order.Number), client.FullName), total, TeamNotificationTexts.OrdersUrl),
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

        var nextStep = !order.HasProducts ? OrderEmails.ClassesCredited
            : order.IsReady ? await WhereToGetItAsync(order, business, cancellationToken)
            : OrderEmails.WillNotifyWhenReady;
        if (order.HasProducts && order.IsReady)
        {
            PushReady(order, nextStep);
        }

        foreach (var email in await FamilyEmailsAsync(order, cancellationToken))
        {
            await SendAsync(
                OrderEmails.Paid(EmailContextOf(order, business, email), nextStep, order.HasProducts, webAppLinks.FamilyOrders()),
                cancellationToken);
        }
    }

    public async Task OrderReadyAsync(Order order, CancellationToken cancellationToken)
    {
        var business = await CurrentBusinessAsync(cancellationToken);
        if (business is null)
        {
            return;
        }

        var whereToGetIt = await WhereToGetItAsync(order, business, cancellationToken);
        PushReady(order, whereToGetIt);
        foreach (var email in await FamilyEmailsAsync(order, cancellationToken))
        {
            await SendAsync(OrderEmails.Ready(EmailContextOf(order, business, email), whereToGetIt, webAppLinks.FamilyOrders()), cancellationToken);
        }
    }

    public async Task OrderCancelledAsync(Order order, OrderCancellationReason reason, CancellationToken cancellationToken)
    {
        var business = await CurrentBusinessAsync(cancellationToken);
        if (business is null)
        {
            return;
        }

        foreach (var email in await FamilyEmailsAsync(order, cancellationToken))
        {
            var emailContext = EmailContextOf(order, business, email);
            var message = reason == OrderCancellationReason.Unpaid
                ? OrderEmails.CancelledUnpaid(emailContext, webAppLinks.FamilyOrders())
                : OrderEmails.CancelledByBranch(emailContext, webAppLinks.FamilyOrders());
            await SendAsync(message, cancellationToken);
        }
    }

    private static OrderEmailContext EmailContextOf(Order order, Business business, string to) =>
        new(
            to,
            order.Number,
            business.Id,
            business.BrandDisplayName,
            [.. order.Lines.Select(line => OrderEmails.Line(line.Quantity, line.Name, line.Total, business.CurrencyCode))],
            OrderEmails.Amount(order.Total, business.CurrencyCode));

    private void PushReady(Order order, string whereToGetIt)
    {
        if (order.ClientId is { } clientId)
        {
            pushPublisher.PublishToFamilies([clientId], new PushMessage(FamilyPushTexts.OrderReadyTitle, whereToGetIt, FamilyPushTexts.OrdersUrl));
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

    private async Task<string> WhereToGetItAsync(Order order, Business business, CancellationToken cancellationToken) =>
        await DeliveryClassNameAsync(order, cancellationToken) is { } classGroupName
            ? OrderEmails.InClass(classGroupName)
            : OrderEmails.PickupAt(business.Name);

    private async Task<IReadOnlyList<string>> FamilyEmailsAsync(Order order, CancellationToken cancellationToken)
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
            return [.. (await identityService.ListAccountsAsync(userIds, cancellationToken)).Select(account => account.Email).Distinct()];
        }

        var clientEmail = (await ClientOfAsync(order, cancellationToken))?.Email;
        return clientEmail is null ? [] : [clientEmail];
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

    private async Task<IReadOnlyList<string>> EmailsOfAsync(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken) =>
        userIds.Count == 0
            ? []
            : [.. (await identityService.ListAccountsAsync(userIds, cancellationToken)).Select(account => account.Email).Distinct()];

    private static IReadOnlySet<string> PermissionsOf(BusinessMember member, Dictionary<Guid, CustomRole> customRoles) =>
        member.CustomRoleId is { } customRoleId
            ? customRoles.TryGetValue(customRoleId, out var customRole) ? MemberRole.Custom(customRole).Permissions : new HashSet<string>()
            : SystemRolePermissions.Of(member.Role);

    private async Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        try
        {
            await emailSender.SendAsync(message, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            LogNotSent(logger, exception, message.Subject);
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Order email not sent: {Subject}")]
    private static partial void LogNotSent(ILogger logger, Exception exception, string subject);
}
