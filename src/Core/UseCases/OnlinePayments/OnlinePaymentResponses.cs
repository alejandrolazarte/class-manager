using ClassManager.Core.Domain.OnlinePayments;

namespace ClassManager.Core.UseCases.OnlinePayments;

public sealed record OnlinePaymentsResponse(
    OnlinePaymentStatus Status,
    bool IsAvailable,
    OnlinePaymentAccountResponse? Account,
    IReadOnlyList<OnlinePaymentAccountResponse> BrandAccounts);

public sealed record OnlinePaymentAccountResponse(long Id, OnlinePaymentStatus Status, IReadOnlyList<string> BranchNames);

public sealed record OnlinePaymentRedirectResponse(Uri Url);
