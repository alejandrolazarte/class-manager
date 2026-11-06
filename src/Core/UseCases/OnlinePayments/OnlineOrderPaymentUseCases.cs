using System.Globalization;
using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.OnlinePayments;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Orders;
using ClassManager.Core.UseCases.Orders;
using ClassManager.Core.UseCases.StudentApp;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Core.UseCases.OnlinePayments;

public sealed record StartStudentAppOrderCheckoutCommand(long OrderId) : ICommand;

public sealed record RecordOnlineOrderPaymentCommand(long OrderId, decimal Amount, [property: AuditPlain] string ExternalPaymentId) : ICommand;

public sealed class StartStudentAppOrderCheckoutUseCase(
    IStudentAppFamilyService studentAppFamily,
    IOrderRepository orderRepository,
    IBusinessRepository businessRepository,
    ISubscriptionRepository subscriptionRepository,
    IOnlineChargingService onlineCharging,
    IOnlinePaymentGateway gateway,
    IWebAppLinks webAppLinks)
    : IUseCase<StartStudentAppOrderCheckoutCommand, OnlinePaymentRedirectResponse>
{
    private const decimal PercentBase = 100m;

    public async Task<Result<OnlinePaymentRedirectResponse>> ExecuteAsync(StartStudentAppOrderCheckoutCommand command, CancellationToken cancellationToken)
    {
        var family = await studentAppFamily.GetAsync(cancellationToken);
        if (family is null)
        {
            return StudentAppFailures.NoAccess();
        }

        var order = await orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
        if (order is null || order.ClientId != family.Access.ClientId || order.Channel != OrderChannel.App || !family.SeesOrderPlacedBy(order.CreatedByUserId))
        {
            return StudentAppFailures.OrderNotFound();
        }

        if (!order.CanBePaidOnline)
        {
            return Result.Conflict<OnlinePaymentRedirectResponse>(OnlinePaymentErrorCodes.NothingToPayOnlineMessage, OnlinePaymentErrorCodes.NothingToPayOnline);
        }

        var account = await onlineCharging.FindChargingAccountAsync(cancellationToken);
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (account is null || business is null)
        {
            return Result.Conflict<OnlinePaymentRedirectResponse>(OnlinePaymentErrorCodes.NotChargingMessage, OnlinePaymentErrorCodes.NotCharging);
        }

        var feePercent = await SalesFeePercentAsync(business.OrganizationId, cancellationToken);
        var checkoutUrl = await gateway.CreateCheckoutAsync(
            new OnlineCheckoutRequest(
                account.ExternalAccountId,
                OnlinePaymentErrorCodes.OrderDescriptionPrefix + order.Number.ToString(CultureInfo.InvariantCulture),
                order.Balance,
                order.CurrencyCode,
                decimal.Floor(order.Balance * feePercent) / PercentBase,
                business.Id,
                order.Id,
                new Uri(webAppLinks.StudentAppOrderPaymentSucceeded()),
                new Uri(webAppLinks.StudentAppOrderPaymentCancelled())),
            cancellationToken);
        return new OnlinePaymentRedirectResponse(checkoutUrl);
    }

    private async Task<decimal> SalesFeePercentAsync(long organizationId, CancellationToken cancellationToken)
    {
        var subscription = await subscriptionRepository.GetCurrentAsync(organizationId, cancellationToken);
        var plans = await subscriptionRepository.ListActivePlansAsync(cancellationToken);
        var plan = plans.FirstOrDefault(candidate => candidate.Code == subscription?.PlanCode);
        var brandFee = await subscriptionRepository.GetSalesFeeOverrideAsync(organizationId, cancellationToken);
        return SalesFees.PercentOf(plan, brandFee);
    }
}

public sealed class RecordOnlineOrderPaymentUseCase(
    IOrderRepository orderRepository,
    IClassPackRepository classPackRepository,
    IClassPackPurchaseRepository purchaseRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    IOrderNotificationService orderNotifications,
    TimeProvider timeProvider)
    : IUseCase<RecordOnlineOrderPaymentCommand, bool>
{
    public async Task<Result<bool>> ExecuteAsync(RecordOnlineOrderPaymentCommand command, CancellationToken cancellationToken)
    {
        if (await orderRepository.HasExternalPaymentAsync(command.ExternalPaymentId, cancellationToken))
        {
            return false;
        }

        var order = await orderRepository.GetForUpdateAsync(command.OrderId, cancellationToken);
        if (order is null)
        {
            return OrderFailures.NotFound();
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var now = timeProvider.GetUtcNow();
        var payment = order.RecordOnlinePayment(command.Amount, command.ExternalPaymentId, today, now);
        if (payment.IsFailure)
        {
            return payment.Error!;
        }

        if (!order.HasProducts && order.Status == OrderStatus.Pending && order.PaymentStatus == OrderPaymentStatus.Paid)
        {
            var delivery = await OrderDeliveries.DeliverAsync(
                order, classPackRepository, purchaseRepository, today, now, deliveredByUserId: null, cancellationToken);
            if (delivery.IsFailure)
            {
                return delivery.Error!;
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        if (order.PaymentStatus == OrderPaymentStatus.Paid)
        {
            await orderNotifications.OrderPaidAsync(order, cancellationToken);
        }

        return true;
    }
}
