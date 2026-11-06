using ClassManager.Core.Domain.OnlinePayments;

namespace ClassManager.Core.Abstractions.Persistence;

public sealed record OnlinePaymentAccountUse(OnlinePaymentAccount Account, IReadOnlyList<string> BranchNames);
