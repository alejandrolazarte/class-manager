using System.Net.Mail;

using ClassManager.Core.Common;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Clients;

public sealed class ClientInvitation : ITenantOwned
{
    public const int EmailMaxLength = 254;
    public const int TokenHashLength = 64;

    public static readonly TimeSpan Lifetime = TimeSpan.FromDays(7);

    private const string EmailFormatMessage = "Email must be a valid address of at most 254 characters.";

    private ClientInvitation()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid? StudentId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string TokenHash { get; private set; } = string.Empty;
    public Guid InvitedByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? AcceptedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public DateTimeOffset? DeclinedAt { get; private set; }

    public static Result<ClientInvitation> Create(
        Guid clientId,
        string? email,
        string tokenHash,
        Guid invitedByUserId,
        DateTimeOffset createdAt,
        Guid? studentId = null)
    {
        var trimmedEmail = email?.Trim() ?? string.Empty;
        if (!IsValidEmail(trimmedEmail))
        {
            return Result.Validation<ClientInvitation>(EmailFormatMessage, fieldName: nameof(Email));
        }

        return new ClientInvitation
        {
            Id = Guid.CreateVersion7(),
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

    private static bool IsValidEmail(string email) =>
        email.Length is > 0 and <= EmailMaxLength
        && MailAddress.TryCreate(email, out var mailAddress)
        && string.Equals(mailAddress.Address, email, StringComparison.OrdinalIgnoreCase);
}
