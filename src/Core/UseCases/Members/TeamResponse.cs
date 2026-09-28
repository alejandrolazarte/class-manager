using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Members;

public sealed record MemberResponse(
    Guid Id,
    string FullName,
    string Email,
    BusinessRole Role,
    Guid? InstructorId,
    bool IsCurrentUser)
{
    public static MemberResponse From(BusinessMember member, UserAccount? account, Guid? currentUserId) =>
        new(
            member.Id,
            account?.FullName ?? string.Empty,
            account?.Email ?? string.Empty,
            member.Role,
            member.InstructorId,
            member.UserId == currentUserId);
}

public sealed record InvitationResponse(
    Guid Id,
    string Email,
    BusinessRole Role,
    Guid? InstructorId,
    DateTimeOffset ExpiresAt)
{
    public static InvitationResponse From(MemberInvitation invitation) =>
        new(invitation.Id, invitation.Email, invitation.Role, invitation.InstructorId, invitation.ExpiresAt);
}

public sealed record TeamResponse(IReadOnlyList<MemberResponse> Members, IReadOnlyList<InvitationResponse> Invitations);
