using ClassManager.Core.Abstractions.OnlinePayments;
using ClassManager.Core.Domain.OnlinePayments;

namespace ClassManager.Infrastructure.OnlinePayments;

internal sealed class UnconfiguredOnlinePaymentGateway : IOnlinePaymentGateway
{
    private const string NotConfiguredMessage = "Stripe Connect is not configured: set Stripe:SecretKey.";

    public Task<string> CreateAccountAsync(OnlinePaymentAccountRequest request, CancellationToken cancellationToken) =>
        throw new InvalidOperationException(NotConfiguredMessage);

    public Task<Uri> CreateOnboardingLinkAsync(OnboardingLinkRequest request, CancellationToken cancellationToken) =>
        throw new InvalidOperationException(NotConfiguredMessage);

    public Task<OnlinePaymentAccountState?> GetAccountAsync(string externalAccountId, CancellationToken cancellationToken) =>
        throw new InvalidOperationException(NotConfiguredMessage);

    public Task<Uri> CreateCheckoutAsync(OnlineCheckoutRequest request, CancellationToken cancellationToken) =>
        throw new InvalidOperationException(NotConfiguredMessage);
}
