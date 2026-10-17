using ClassManager.Core.Common;
using ClassManager.Core.Domain.Users;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Clients;

public sealed class Client : ITenantOwned
{
    public const int NotesMaxLength = 1000;

    private const string NotesLengthMessage = "Notes must be at most 1000 characters.";

    private Client()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string? Notes { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? RegisteredByUserId { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public static Result<Client> Create(
        string? fullName,
        PhoneNumber phoneNumber,
        string? email,
        string? notes,
        DateTimeOffset createdAt)
    {
        var contact = User.Create(fullName, email, createdAt);
        if (contact.IsFailure)
        {
            return contact.Error!;
        }

        var validNotes = ValidNotes(notes);
        if (validNotes.IsFailure)
        {
            return validNotes.Error!;
        }

        contact.Value!.ChangePhoneNumber(phoneNumber);
        return new Client
        {
            Id = Guid.CreateVersion7(),
            Notes = validNotes.Value,
            CreatedAt = createdAt.ToUniversalTime(),
            UserId = contact.Value.Id,
            User = contact.Value,
        };
    }

    public static Result<Client> CreateFor(User contact, PhoneNumber phoneNumber, string? notes, DateTimeOffset createdAt)
    {
        var validNotes = ValidNotes(notes);
        if (validNotes.IsFailure)
        {
            return validNotes.Error!;
        }

        contact.ChangePhoneNumber(phoneNumber);
        return new Client
        {
            Id = Guid.CreateVersion7(),
            Notes = validNotes.Value,
            CreatedAt = createdAt.ToUniversalTime(),
            UserId = contact.Id,
            User = contact,
        };
    }

    public User MoveTo(User contact)
    {
        var previousContact = User;
        contact.FillBlanksFrom(previousContact);
        User = contact;
        UserId = contact.Id;
        return previousContact;
    }

    public Result Update(string? fullName, PhoneNumber phoneNumber, string? email, string? notes)
    {
        var rename = User.Rename(fullName);
        if (rename.IsFailure)
        {
            return rename;
        }

        var emailChange = User.ChangeEmail(email);
        if (emailChange.IsFailure)
        {
            return emailChange;
        }

        var validNotes = ValidNotes(notes);
        if (validNotes.IsFailure)
        {
            return Result.Failure(validNotes.Error!);
        }

        User.ChangePhoneNumber(phoneNumber);
        Notes = validNotes.Value;
        return Result.Success();
    }

    public void RecordRegisteredBy(Guid userId) => RegisteredByUserId = userId;

    private static Result<string?> ValidNotes(string? notes)
    {
        var trimmedNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        return trimmedNotes?.Length > NotesMaxLength
            ? Result.Validation<string?>(NotesLengthMessage, fieldName: nameof(Notes))
            : trimmedNotes;
    }
}
