using System.Collections.Concurrent;
using ClassManager.Core.Abstractions.OnlinePayments;
using ClassManager.Core.Domain.OnlinePayments;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed class FakeOnlinePaymentGateway : IOnlinePaymentGateway
{
    public const string OnboardingHost = "https://connect.stripe.test/";
    public const string CheckoutHost = "https://checkout.stripe.test/";

    private const string AccountIdPrefix = "acct_";

    private static readonly OnlinePaymentAccountState NewAccountState = new(DetailsSubmitted: false, ChargesEnabled: false, PayoutsEnabled: false);

    private readonly ConcurrentDictionary<string, OnlinePaymentAccountState> _accounts = new(StringComparer.Ordinal);
    private readonly ConcurrentQueue<OnlinePaymentAccountRequest> _accountRequests = new();
    private readonly ConcurrentQueue<OnboardingLinkRequest> _onboardingLinkRequests = new();
    private readonly ConcurrentQueue<OnlineCheckoutRequest> _checkoutRequests = new();

    public static OnlinePaymentAccountState ActiveAccountState { get; } = new(DetailsSubmitted: true, ChargesEnabled: true, PayoutsEnabled: true);

    public void Set(string externalAccountId, OnlinePaymentAccountState state) => _accounts[externalAccountId] = state;

    public IReadOnlyList<OnlinePaymentAccountRequest> AccountsCreatedFor(long businessId) =>
        [.. _accountRequests.Where(request => request.BusinessId == businessId)];

    public IReadOnlyList<OnboardingLinkRequest> OnboardingLinksFor(string externalAccountId) =>
        [.. _onboardingLinkRequests.Where(request => request.ExternalAccountId == externalAccountId)];

    public IReadOnlyList<OnlineCheckoutRequest> CheckoutsFor(long orderId) =>
        [.. _checkoutRequests.Where(request => request.OrderId == orderId)];

    public static Uri CheckoutUrlOf(long orderId) => new(CheckoutHost + orderId);

    public static Uri OnboardingUrlOf(string externalAccountId) => new(OnboardingHost + externalAccountId);

    public Task<string> CreateAccountAsync(OnlinePaymentAccountRequest request, CancellationToken cancellationToken)
    {
        _accountRequests.Enqueue(request);
        var externalAccountId = AccountIdPrefix + Guid.NewGuid().ToString("N");
        _accounts[externalAccountId] = NewAccountState;
        return Task.FromResult(externalAccountId);
    }

    public Task<Uri> CreateOnboardingLinkAsync(OnboardingLinkRequest request, CancellationToken cancellationToken)
    {
        _onboardingLinkRequests.Enqueue(request);
        return Task.FromResult(OnboardingUrlOf(request.ExternalAccountId));
    }

    public Task<OnlinePaymentAccountState?> GetAccountAsync(string externalAccountId, CancellationToken cancellationToken) =>
        Task.FromResult(_accounts.TryGetValue(externalAccountId, out var state) ? state : null);

    public Task<Uri> CreateCheckoutAsync(OnlineCheckoutRequest request, CancellationToken cancellationToken)
    {
        _checkoutRequests.Enqueue(request);
        return Task.FromResult(CheckoutUrlOf(request.OrderId));
    }
}
