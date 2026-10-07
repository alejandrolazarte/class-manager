using System.Net.Mail;

using ClassManager.Core.Common;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Students;

public sealed class Student : ITenantOwned
{
    public const int FullNameMinLength = 2;
    public const int FullNameMaxLength = 120;
    public const int NotesMaxLength = 1000;
    public const int EmailMaxLength = 254;

    public static readonly DateOnly EarliestBirthDate = new(1900, 1, 1);

    private const string FullNameLengthMessage = "Full name must be between 2 and 120 characters.";
    private const string BirthDateRangeMessage = "Birth date must be between 1900-01-01 and today.";
    private const string NotesLengthMessage = "Notes must be at most 1000 characters.";
    private const string EmailFormatMessage = "Email must be a valid address of at most 254 characters.";

    private Student()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ClientId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public DateOnly? BirthDate { get; private set; }
    public string? Notes { get; private set; }
    public string? Email { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static Result<Student> Create(
        Guid clientId,
        string? fullName,
        DateOnly? birthDate,
        string? notes,
        DateOnly today,
        DateTimeOffset createdAt,
        string? email = null)
    {
        var trimmedFullName = fullName?.Trim() ?? string.Empty;
        if (trimmedFullName.Length is < FullNameMinLength or > FullNameMaxLength)
        {
            return Result.Validation<Student>(FullNameLengthMessage, fieldName: nameof(FullName));
        }

        if (!IsValidBirthDate(birthDate, today))
        {
            return Result.Validation<Student>(BirthDateRangeMessage, fieldName: nameof(BirthDate));
        }

        var trimmedNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        if (trimmedNotes?.Length > NotesMaxLength)
        {
            return Result.Validation<Student>(NotesLengthMessage, fieldName: nameof(Notes));
        }

        var trimmedEmail = TrimToNull(email);
        if (trimmedEmail is not null && !IsValidEmail(trimmedEmail))
        {
            return Result.Validation<Student>(EmailFormatMessage, fieldName: nameof(Email));
        }

        return new Student
        {
            Id = Guid.CreateVersion7(),
            ClientId = clientId,
            FullName = trimmedFullName,
            BirthDate = birthDate,
            Notes = trimmedNotes,
            Email = trimmedEmail,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }

    public bool HasEmail(string? email) => string.Equals(TrimToNull(email), Email, StringComparison.OrdinalIgnoreCase);

    public Result ChangeEmail(string? email)
    {
        var trimmedEmail = TrimToNull(email);
        if (trimmedEmail is not null && !IsValidEmail(trimmedEmail))
        {
            return Result.Validation(EmailFormatMessage, fieldName: nameof(Email));
        }

        Email = trimmedEmail;
        return Result.Success();
    }

    public Result ChangeBirthDate(DateOnly? birthDate, DateOnly today)
    {
        if (!IsValidBirthDate(birthDate, today))
        {
            return Result.Validation(BirthDateRangeMessage, fieldName: nameof(BirthDate));
        }

        BirthDate = birthDate;
        return Result.Success();
    }

    private static bool IsValidBirthDate(DateOnly? birthDate, DateOnly today) =>
        birthDate is null || (birthDate >= EarliestBirthDate && birthDate <= today);

    private static string? TrimToNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static bool IsValidEmail(string email) =>
        email.Length <= EmailMaxLength
        && MailAddress.TryCreate(email, out var mailAddress)
        && string.Equals(mailAddress.Address, email, StringComparison.OrdinalIgnoreCase);
}
