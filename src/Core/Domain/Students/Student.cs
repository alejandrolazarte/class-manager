using ClassManager.Core.Common;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Students;

public sealed class Student : ITenantOwned
{
    public const int FullNameMinLength = 2;
    public const int FullNameMaxLength = 120;
    public const int NotesMaxLength = 1000;

    public static readonly DateOnly EarliestBirthDate = new(1900, 1, 1);

    private const string FullNameLengthMessage = "Full name must be between 2 and 120 characters.";
    private const string BirthDateRangeMessage = "Birth date must be between 1900-01-01 and today.";
    private const string NotesLengthMessage = "Notes must be at most 1000 characters.";

    private Student()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ClientId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public DateOnly? BirthDate { get; private set; }
    public string? Notes { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static Result<Student> Create(
        Guid clientId,
        string? fullName,
        DateOnly? birthDate,
        string? notes,
        DateOnly today,
        DateTimeOffset createdAt)
    {
        var trimmedFullName = fullName?.Trim() ?? string.Empty;
        if (trimmedFullName.Length is < FullNameMinLength or > FullNameMaxLength)
        {
            return Result.Validation<Student>(FullNameLengthMessage, fieldName: nameof(FullName));
        }

        if (birthDate < EarliestBirthDate || birthDate > today)
        {
            return Result.Validation<Student>(BirthDateRangeMessage, fieldName: nameof(BirthDate));
        }

        var trimmedNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        if (trimmedNotes?.Length > NotesMaxLength)
        {
            return Result.Validation<Student>(NotesLengthMessage, fieldName: nameof(Notes));
        }

        return new Student
        {
            Id = Guid.CreateVersion7(),
            ClientId = clientId,
            FullName = trimmedFullName,
            BirthDate = birthDate,
            Notes = trimmedNotes,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }
}
