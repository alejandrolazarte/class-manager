using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Core.Operators;

public sealed record PlatformTeamMember(
    long MemberId,
    long BranchId,
    string BranchName,
    long UserId,
    Guid? AccountId,
    string FullName,
    string? Email,
    BusinessRole Role);
