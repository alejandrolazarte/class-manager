using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Members;

public sealed record MemberResponse(
    long Id,
    string FullName,
    string Email,
    BusinessRole Role,
    long? CustomRoleId,
    long? InstructorId,
    bool IsCurrentUser,
    bool IsBrandOwner,
    string? PhotoUrl = null)
{
    public static MemberResponse From(BusinessMember member, UserAccount? account, Guid? currentAccountId, bool isBrandOwner, string? photoUrl = null) =>
        new(
            member.Id,
            account?.FullName ?? string.Empty,
            account?.Email ?? string.Empty,
            member.Role,
            member.CustomRoleId,
            member.InstructorId,
            member.User.AccountId == currentAccountId,
            isBrandOwner,
            photoUrl);
}

public sealed record InvitationResponse(
    long Id,
    string Email,
    BusinessRole Role,
    long? CustomRoleId,
    long? InstructorId,
    DateTimeOffset ExpiresAt)
{
    public static InvitationResponse From(MemberInvitation invitation) =>
        new(invitation.Id, invitation.Email, invitation.Role, invitation.CustomRoleId, invitation.InstructorId, invitation.ExpiresAt);
}

public sealed record TeamResponse(IReadOnlyList<MemberResponse> Members, IReadOnlyList<InvitationResponse> Invitations);
