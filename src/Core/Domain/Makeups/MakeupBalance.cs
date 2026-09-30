namespace ClassManager.Core.Domain.Makeups;

public sealed record MakeupBalance(IReadOnlyList<MakeupCredit> Available, int UncoveredBookings);
