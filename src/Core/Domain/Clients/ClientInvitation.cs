using System.Net.Mail;
using ClassManager.Core.Common;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Clients;

public sealed class ClientInvitation : ITenantOwned, IAuditable
{
    public const int EmailMaxLength = 254;
    public const int TokenHashLength = 64;

    public static readonly TimeSpan Lifetime = TimeSpan.FromDays(7);

    private const string EmailFormatMessage = "Email must be a valid address of at most 254 characters.";

    private ClientInvitation()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public long ClientId { get; private set; }
    public long? StudentId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string TokenHash { get; private set; } = string.Empty;
    public long? InvitedByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? AcceptedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public DateTimeOffset? DeclinedAt { get; private set; }
    public string? GuardianEmail { get; private set; }
    public string? GuardianTokenHash { get; private set; }
    public DateTimeOffset? GuardianConsentedAt { get; private set; }
    public long? ContactAdultConfirmedByUserId { get; private set; }
    public DateTimeOffset? ContactAdultConfirmedAt { get; private set; }

    public bool AwaitsGuardianConsent => GuardianTokenHash is not null && GuardianConsentedAt is null;

    public static Result<ClientInvitation> Create(
        long clientId,
        string? email,
        string tokenHash,
        long? invitedByUserId,
        DateTimeOffset createdAt,
        long? studentId = null)
    {
        var trimmedEmail = email?.Trim() ?? string.Empty;
        if (!IsValidEmail(trimmedEmail))
        {
            return Result.Validation<ClientInvitation>(EmailFormatMessage, fieldName: nameof(Email));
        }

        return new ClientInvitation
        {
            ClientId = clientId,
            StudentId = studentId,
            Email = trimmedEmail,
            TokenHash = tokenHash,
            InvitedByUserId = invitedByUserId,
            CreatedAt = createdAt.ToUniversalTime(),
            ExpiresAt = (createdAt + Lifetime).ToUniversalTime(),
        };
    }

    public bool IsPendingAt(DateTimeOffset now) => AcceptedAt is null && RevokedAt is null && DeclinedAt is null && ExpiresAt > now;

    public void Accept(DateTimeOffset acceptedAt) => AcceptedAt = acceptedAt.ToUniversalTime();

    public void Revoke(DateTimeOffset revokedAt) => RevokedAt = revokedAt.ToUniversalTime();

    public void Decline(DateTimeOffset declinedAt) => DeclinedAt = declinedAt.ToUniversalTime();

    public void RequireGuardianConsent(string guardianEmail, string guardianTokenHash)
    {
        GuardianEmail = guardianEmail;
        GuardianTokenHash = guardianTokenHash;
    }

    public void ConfirmContactIsAdult(long? confirmedByUserId, DateTimeOffset confirmedAt)
    {
        ContactAdultConfirmedByUserId = confirmedByUserId;
        ContactAdultConfirmedAt = confirmedAt.ToUniversalTime();
    }

    public void KeepContactAdultConfirmationOf(ClientInvitation previousInvitation)
    {
        ContactAdultConfirmedByUserId = previousInvitation.ContactAdultConfirmedByUserId;
        ContactAdultConfirmedAt = previousInvitation.ContactAdultConfirmedAt;
    }

    public void ConsentByGuardian(string tokenHash, DateTimeOffset consentedAt)
    {
        GuardianConsentedAt = consentedAt.ToUniversalTime();
        TokenHash = tokenHash;
        ExpiresAt = (consentedAt + Lifetime).ToUniversalTime();
    }

    private static bool IsValidEmail(string email) =>
        email.Length is > 0 and <= EmailMaxLength
        && MailAddress.TryCreate(email, out var mailAddress)
        && string.Equals(mailAddress.Address, email, StringComparison.OrdinalIgnoreCase);
}
