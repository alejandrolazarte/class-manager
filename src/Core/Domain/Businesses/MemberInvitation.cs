using System.Net.Mail;

using ClassManager.Core.Common;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Businesses;

public sealed class MemberInvitation : ITenantOwned
{
    public const int EmailMaxLength = 254;
    public const int TokenHashLength = 64;

    public static readonly TimeSpan Lifetime = TimeSpan.FromDays(7);

    private const string EmailFormatMessage = "Email must be a valid address of at most 254 characters.";

    private MemberInvitation()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public BusinessRole Role { get; private set; }
    public Guid? CustomRoleId { get; private set; }
    public Guid? InstructorId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public Guid InvitedByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? AcceptedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }

    public static Result<MemberInvitation> Create(
        string? email,
        MemberRole role,
        Guid? instructorId,
        string tokenHash,
        Guid invitedByUserId,
        DateTimeOffset createdAt)
    {
        var trimmedEmail = email?.Trim() ?? string.Empty;
        if (!IsValidEmail(trimmedEmail))
        {
            return Result.Validation<MemberInvitation>(EmailFormatMessage, fieldName: nameof(Email));
        }

        if (BusinessMember.ValidateRole(role, instructorId) is { } roleError)
        {
            return roleError;
        }

        return new MemberInvitation
        {
            Id = Guid.CreateVersion7(),
            Email = trimmedEmail,
            Role = role.Role,
            CustomRoleId = role.CustomRoleId,
            InstructorId = instructorId,
            TokenHash = tokenHash,
            InvitedByUserId = invitedByUserId,
            CreatedAt = createdAt.ToUniversalTime(),
            ExpiresAt = (createdAt + Lifetime).ToUniversalTime(),
        };
    }

    public bool IsPendingAt(DateTimeOffset now) => AcceptedAt is null && RevokedAt is null && ExpiresAt > now;

    public bool IsOpen => AcceptedAt is null && RevokedAt is null;

    public void Renew(string tokenHash, DateTimeOffset renewedAt)
    {
        TokenHash = tokenHash;
        ExpiresAt = (renewedAt + Lifetime).ToUniversalTime();
    }

    public void Accept(DateTimeOffset acceptedAt) => AcceptedAt = acceptedAt.ToUniversalTime();

    public void Revoke(DateTimeOffset revokedAt) => RevokedAt = revokedAt.ToUniversalTime();

    private static bool IsValidEmail(string email) =>
        email.Length is > 0 and <= EmailMaxLength
        && MailAddress.TryCreate(email, out var mailAddress)
        && string.Equals(mailAddress.Address, email, StringComparison.OrdinalIgnoreCase);
}
