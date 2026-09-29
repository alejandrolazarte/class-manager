using System.Net.Mail;
using ClassManager.Core.Common;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Clients;

public sealed class Client : ITenantOwned
{
    public const int FullNameMinLength = 2;
    public const int FullNameMaxLength = 120;
    public const int EmailMaxLength = 254;
    public const int NotesMaxLength = 1000;

    private const string FullNameLengthMessage = "Full name must be between 2 and 120 characters.";
    private const string EmailFormatMessage = "Email must be a valid address of at most 254 characters.";
    private const string NotesLengthMessage = "Notes must be at most 1000 characters.";

    private Client()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public PhoneNumber PhoneNumber { get; private set; } = null!;
    public string? Email { get; private set; }
    public string? Notes { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? RegisteredByUserId { get; private set; }

    public static Result<Client> Create(
        string? fullName,
        PhoneNumber phoneNumber,
        string? email,
        string? notes,
        DateTimeOffset createdAt)
    {
        var trimmedFullName = fullName?.Trim() ?? string.Empty;
        if (trimmedFullName.Length is < FullNameMinLength or > FullNameMaxLength)
        {
            return Result.Validation<Client>(FullNameLengthMessage, fieldName: nameof(FullName));
        }

        var trimmedEmail = TrimToNull(email);
        if (trimmedEmail is not null && !IsValidEmail(trimmedEmail))
        {
            return Result.Validation<Client>(EmailFormatMessage, fieldName: nameof(Email));
        }

        var trimmedNotes = TrimToNull(notes);
        if (trimmedNotes?.Length > NotesMaxLength)
        {
            return Result.Validation<Client>(NotesLengthMessage, fieldName: nameof(Notes));
        }

        return new Client
        {
            Id = Guid.CreateVersion7(),
            FullName = trimmedFullName,
            PhoneNumber = phoneNumber,
            Email = trimmedEmail,
            Notes = trimmedNotes,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }

    public void RecordRegisteredBy(Guid userId) => RegisteredByUserId = userId;

    private static string? TrimToNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static bool IsValidEmail(string email) =>
        email.Length <= EmailMaxLength
        && MailAddress.TryCreate(email, out var mailAddress)
        && string.Equals(mailAddress.Address, email, StringComparison.OrdinalIgnoreCase);
}
