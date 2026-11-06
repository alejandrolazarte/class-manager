using System.Globalization;
using ClassManager.Core.Abstractions.OnlinePayments;
using ClassManager.Core.Domain.OnlinePayments;
using Stripe;

namespace ClassManager.Infrastructure.OnlinePayments;

internal sealed class StripeOnlinePaymentGateway(StripeClient client) : IOnlinePaymentGateway
{
    public const string FullDashboard = "full";
    public const string AccountPaysFees = "account";
    public const string StripeCoversLosses = "stripe";
    public const string StripeCollectsRequirements = "stripe";
    public const string AccountOnboarding = "account_onboarding";
    public const string BusinessIdMetadataKey = "business_id";
    public const string OrganizationIdMetadataKey = "organization_id";

    private const string ResourceMissingCode = "resource_missing";

    public async Task<string> CreateAccountAsync(OnlinePaymentAccountRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var account = await client.V1.Accounts.CreateAsync(
            new AccountCreateOptions
            {
                Country = request.CountryCode,
                Email = request.Email,
                Controller = new AccountControllerOptions
                {
                    StripeDashboard = new AccountControllerStripeDashboardOptions { Type = FullDashboard },
                    Fees = new AccountControllerFeesOptions { Payer = AccountPaysFees },
                    Losses = new AccountControllerLossesOptions { Payments = StripeCoversLosses },
                    RequirementCollection = StripeCollectsRequirements,
                },
                Metadata = new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    [BusinessIdMetadataKey] = request.BusinessId.ToString(CultureInfo.InvariantCulture),
                    [OrganizationIdMetadataKey] = request.OrganizationId.ToString(CultureInfo.InvariantCulture),
                },
            },
            cancellationToken: cancellationToken);
        return account.Id;
    }

    public async Task<Uri> CreateOnboardingLinkAsync(OnboardingLinkRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var link = await client.V1.AccountLinks.CreateAsync(
            new AccountLinkCreateOptions
            {
                Account = request.ExternalAccountId,
                RefreshUrl = request.RefreshUrl.ToString(),
                ReturnUrl = request.ReturnUrl.ToString(),
                Type = AccountOnboarding,
            },
            cancellationToken: cancellationToken);
        return new Uri(link.Url);
    }

    public async Task<OnlinePaymentAccountState?> GetAccountAsync(string externalAccountId, CancellationToken cancellationToken)
    {
        try
        {
            var account = await client.V1.Accounts.GetAsync(externalAccountId, cancellationToken: cancellationToken);
            return new OnlinePaymentAccountState(account.DetailsSubmitted, account.ChargesEnabled, account.PayoutsEnabled);
        }
        catch (StripeException exception) when (exception.StripeError?.Code == ResourceMissingCode)
        {
            return null;
        }
    }
}
