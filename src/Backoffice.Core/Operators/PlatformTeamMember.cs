using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Core.Operators;

public sealed record PlatformTeamMember(
    Guid MemberId,
    Guid BranchId,
    string BranchName,
    Guid UserId,
    Guid? AccountId,
    string FullName,
    string? Email,
    BusinessRole Role);
