using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Subscriptions.Billing;

namespace ClassManager.Core.UseCases.Subscriptions;

public sealed record OpenBillingPortalCommand : ICommand;

public sealed class OpenBillingPortalUseCase(
    IBusinessRepository businessRepository,
    ISubscriptionRepository subscriptionRepository,
    IBillingGateway billingGateway,
    IWebAppLinks webAppLinks)
    : IUseCase<OpenBillingPortalCommand, BillingRedirectResponse>
{
    public async Task<Result<BillingRedirectResponse>> ExecuteAsync(OpenBillingPortalCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return Result.Unauthorized<BillingRedirectResponse>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);
        }

        var customer = await subscriptionRepository.GetBillingCustomerAsync(business.OrganizationId, cancellationToken);
        if (customer is null)
        {
            return Result.NotFound<BillingRedirectResponse>(BillingErrorCodes.NoCustomerMessage, BillingErrorCodes.NoCustomer);
        }

        var portalUrl = await billingGateway.CreatePortalSessionAsync(
            customer.ExternalCustomerId,
            new Uri(webAppLinks.PlanSettings()),
            cancellationToken);
        return new BillingRedirectResponse(portalUrl);
    }
}
