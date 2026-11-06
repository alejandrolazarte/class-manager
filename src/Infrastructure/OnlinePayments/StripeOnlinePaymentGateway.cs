using System.Globalization;
using ClassManager.Core.Abstractions.OnlinePayments;
using ClassManager.Core.Domain.OnlinePayments;
using Stripe;
using Stripe.Checkout;

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
    public const string OrderIdMetadataKey = "order_id";
    public const string PaymentMode = "payment";
    public const decimal MinorUnitsPerMajorUnit = 100m;

    private const int SingleUnit = 1;

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

    public async Task<Uri> CreateCheckoutAsync(OnlineCheckoutRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var metadata = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [BusinessIdMetadataKey] = request.BusinessId.ToString(CultureInfo.InvariantCulture),
            [OrderIdMetadataKey] = request.OrderId.ToString(CultureInfo.InvariantCulture),
        };
        var platformFee = MinorUnitsOf(request.PlatformFee);
        var session = await client.V1.Checkout.Sessions.CreateAsync(
            new SessionCreateOptions
            {
                Mode = PaymentMode,
                LineItems =
                [
                    new SessionLineItemOptions
                    {
                        Quantity = SingleUnit,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = request.CurrencyCode.ToLowerInvariant(),
                            UnitAmount = MinorUnitsOf(request.Amount),
                            ProductData = new SessionLineItemPriceDataProductDataOptions { Name = request.Description },
                        },
                    },
                ],
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    ApplicationFeeAmount = platformFee > 0 ? platformFee : null,
                    Metadata = metadata,
                },
                Metadata = metadata,
                SuccessUrl = request.SuccessUrl.ToString(),
                CancelUrl = request.CancelUrl.ToString(),
            },
            new RequestOptions { StripeAccount = request.ExternalAccountId },
            cancellationToken);
        return new Uri(session.Url);
    }

    public static long MinorUnitsOf(decimal amount) => (long)decimal.Round(amount * MinorUnitsPerMajorUnit);

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
