using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Subscriptions;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Records;
using ClassManager.Subscriptions.Billing;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Core.UseCases.Subscriptions;

public sealed record StartPlanCheckoutCommand([property: AuditPlain] string PlanCode) : ICommand;

public sealed class StartPlanCheckoutUseCase(
    IBusinessRepository businessRepository,
    ISubscriptionRepository subscriptionRepository,
    ISelfServiceBillingService selfServiceBillingService,
    IBillingGateway billingGateway,
    IIdentityService identityService,
    ICurrentUser currentUser,
    IWebAppLinks webAppLinks,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<StartPlanCheckoutCommand, BillingRedirectResponse>
{
    public async Task<Result<BillingRedirectResponse>> ExecuteAsync(StartPlanCheckoutCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<BillingRedirectResponse>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        if (!SelfServiceBilling.PlanCodes.Contains(command.PlanCode))
        {
            return Result.Validation<BillingRedirectResponse>(
                BillingErrorCodes.PlanNotAvailableMessage,
                BillingErrorCodes.PlanNotAvailable,
                nameof(command.PlanCode));
        }

        var subscription = await subscriptionRepository.GetCurrentAsync(business.OrganizationId, cancellationToken);
        if (subscription is null)
        {
            return Result.NotFound<BillingRedirectResponse>(GetOrganizationSubscriptionUseCase.SubscriptionNotFoundMessage);
        }

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        if (subscription.ExternalSubscriptionId is not null && subscription.IsActiveOn(today))
        {
            return Result.Conflict<BillingRedirectResponse>(BillingErrorCodes.AlreadySubscribedMessage, BillingErrorCodes.AlreadySubscribed);
        }

        var activeOffer = await selfServiceBillingService.FindActiveOfferAsync(business.OrganizationId, cancellationToken);
        var offer = activeOffer?.PlanCode == command.PlanCode ? activeOffer : null;
        var access = offer is null
            ? await selfServiceBillingService.GetAccessAsync(subscription, cancellationToken)
            : await selfServiceBillingService.GetAccessAsync(offer, cancellationToken);
        if (access is SelfServiceBillingAccess.NotInPlan)
        {
            return FeatureErrors.NotInPlan(Features.SelfServiceBilling);
        }

        if (access is SelfServiceBillingAccess.CountryNotSupported)
        {
            return Result.Forbidden<BillingRedirectResponse>(BillingErrorCodes.NotAvailableMessage, BillingErrorCodes.NotAvailable);
        }

        var plans = await subscriptionRepository.ListActivePlansAsync(cancellationToken);
        if (plans.FirstOrDefault(plan => plan.Code == command.PlanCode) is not { ListPrice: { } listPrice } chosenPlan)
        {
            return Result.Validation<BillingRedirectResponse>(
                BillingErrorCodes.PlanNotAvailableMessage,
                BillingErrorCodes.PlanNotAvailable,
                nameof(command.PlanCode));
        }

        var externalCustomerId = await FindOrCreateCustomerAsync(business.OrganizationId, cancellationToken);
        if (externalCustomerId is null)
        {
            return Result.Unauthorized<BillingRedirectResponse>(BillingErrorCodes.NoAccountMessage);
        }

        var checkoutUrl = await billingGateway.CreateCheckoutAsync(
            new BillingCheckoutRequest(
                business.OrganizationId,
                externalCustomerId,
                SelfServiceBilling.PriceOf(chosenPlan, offer?.Price ?? listPrice),
                FirstChargeAt(subscription, today),
                new Uri(webAppLinks.PlanCheckoutSucceeded()),
                new Uri(webAppLinks.PlanCheckoutCancelled())),
            cancellationToken);

        return new BillingRedirectResponse(checkoutUrl);
    }

    private async Task<string?> FindOrCreateCustomerAsync(long organizationId, CancellationToken cancellationToken)
    {
        if (await subscriptionRepository.GetBillingCustomerAsync(organizationId, cancellationToken) is { } existingCustomer)
        {
            return existingCustomer.ExternalCustomerId;
        }

        if (currentUser.AccountId is not { } accountId
            || await identityService.ListAccountsAsync([accountId], cancellationToken) is not [var account, ..])
        {
            return null;
        }

        var externalCustomerId = await billingGateway.CreateCustomerAsync(
            new BillingCustomerRequest(organizationId, account.FullName, account.Email, account.PreferredLanguage),
            cancellationToken);
        subscriptionRepository.AddBillingCustomer(BillingCustomer.Create(organizationId, externalCustomerId));
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return externalCustomerId;
    }

    private static DateTimeOffset? FirstChargeAt(Subscription subscription, DateOnly today) =>
        subscription.PlanCode == PlanCodes.Free && subscription.IsActiveOn(today) && subscription.ExpiredOn is { } lastTrialDay
            ? new DateTimeOffset(lastTrialDay.AddDays(1).ToDateTime(TimeOnly.MinValue), TimeSpan.Zero)
            : null;
}
