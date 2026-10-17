using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Users;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Students;

public sealed class Student : ITenantOwned
{
    public const int NotesMaxLength = 1000;

    private const string NotesLengthMessage = "Notes must be at most 1000 characters.";

    private Student()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ClientId { get; private set; }
    public string? Notes { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public static Result<Student> Create(
        Guid clientId,
        string? fullName,
        DateOnly? birthDate,
        string? notes,
        DateOnly today,
        DateTimeOffset createdAt,
        string? email = null)
    {
        var person = User.Create(fullName, null, createdAt);
        if (person.IsFailure)
        {
            return person.Error!;
        }

        var details = PersonDetails(person.Value!, birthDate, notes, today);
        if (details.IsFailure)
        {
            return details.Error!;
        }

        var emailChange = person.Value!.ChangeEmail(email);
        if (emailChange.IsFailure)
        {
            return emailChange.Error!;
        }

        return New(clientId, person.Value, details.Value, createdAt);
    }

    public static Result<Student> CreateAttendingContact(
        Client client,
        DateOnly? birthDate,
        string? notes,
        DateOnly today,
        DateTimeOffset createdAt) =>
        CreateFor(client.Id, client.User, birthDate, notes, today, createdAt);

    public static Result<Student> CreateFor(
        Guid clientId,
        User person,
        DateOnly? birthDate,
        string? notes,
        DateOnly today,
        DateTimeOffset createdAt)
    {
        var validNotes = ValidNotes(notes);
        if (validNotes.IsFailure)
        {
            return validNotes.Error!;
        }

        var birthDateChange = birthDate is null ? Result.Success() : person.ChangeBirthDate(birthDate, today);
        return birthDateChange.IsFailure ? birthDateChange.Error! : New(clientId, person, validNotes.Value, createdAt);
    }

    public bool IsTheContactOf(Client client) => UserId == client.UserId;

    public void ChangePerson(User person)
    {
        UserId = person.Id;
        User = person;
    }

    private static Result<string?> PersonDetails(User person, DateOnly? birthDate, string? notes, DateOnly today)
    {
        var birthDateChange = person.ChangeBirthDate(birthDate, today);
        return birthDateChange.IsFailure ? birthDateChange.Error! : ValidNotes(notes);
    }

    private static Result<string?> ValidNotes(string? notes)
    {
        var trimmedNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        return trimmedNotes?.Length > NotesMaxLength
            ? Result.Validation<string?>(NotesLengthMessage, fieldName: nameof(Notes))
            : trimmedNotes;
    }

    private static Student New(Guid clientId, User person, string? notes, DateTimeOffset createdAt) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            ClientId = clientId,
            Notes = notes,
            CreatedAt = createdAt.ToUniversalTime(),
            UserId = person.Id,
            User = person,
        };
}
