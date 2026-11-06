using ClassManager.Core.Domain.OnlinePayments;

namespace ClassManager.Core.Abstractions.OnlinePayments;

public interface IOnlinePaymentGateway
{
    Task<string> CreateAccountAsync(OnlinePaymentAccountRequest request, CancellationToken cancellationToken);

    Task<Uri> CreateOnboardingLinkAsync(OnboardingLinkRequest request, CancellationToken cancellationToken);

    Task<OnlinePaymentAccountState?> GetAccountAsync(string externalAccountId, CancellationToken cancellationToken);
}
