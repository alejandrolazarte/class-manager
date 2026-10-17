using System.Net.Mail;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Clients;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Users;

public sealed class User : ITenantOwned, ISoftDeletable
{
    public const int FullNameMinLength = 2;
    public const int FullNameMaxLength = 120;
    public const int EmailMaxLength = 254;

    public static readonly DateOnly EarliestBirthDate = new(1900, 1, 1);

    private const string FullNameLengthMessage = "Full name must be between 2 and 120 characters.";
    private const string EmailFormatMessage = "Email must be a valid address of at most 254 characters.";
    private const string BirthDateRangeMessage = "Birth date must be between 1900-01-01 and today.";

    private User()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public Guid? AccountId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public Guid? DeletedByUserId { get; private set; }

    public static Result<User> Create(string? fullName, string? email, DateTimeOffset createdAt)
    {
        var user = new User
        {
            Id = Guid.CreateVersion7(),
            CreatedAt = createdAt.ToUniversalTime(),
        };
        var rename = user.Rename(fullName);
        if (rename.IsFailure)
        {
            return rename.Error!;
        }

        var emailChange = user.ChangeEmail(email);
        return emailChange.IsFailure ? emailChange.Error! : user;
    }

    public static User CreateForAccount(Guid accountId, string fullName, string? email, DateTimeOffset createdAt) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            FullName = fullName.Trim(),
            Email = TrimToNull(email),
            AccountId = accountId,
            CreatedAt = createdAt.ToUniversalTime(),
        };

    public Result Rename(string? fullName)
    {
        var trimmedFullName = fullName?.Trim() ?? string.Empty;
        if (trimmedFullName.Length is < FullNameMinLength or > FullNameMaxLength)
        {
            return Result.Validation(FullNameLengthMessage, fieldName: nameof(FullName));
        }

        FullName = trimmedFullName;
        return Result.Success();
    }

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

    public void ChangePhoneNumber(PhoneNumber? phoneNumber) => PhoneNumber = phoneNumber;

    public static bool IsValidBirthDate(DateOnly? birthDate, DateOnly today) =>
        birthDate is not { } date || (date >= EarliestBirthDate && date <= today);

    public Result ChangeBirthDate(DateOnly? birthDate, DateOnly today)
    {
        if (!IsValidBirthDate(birthDate, today))
        {
            return Result.Validation(BirthDateRangeMessage, fieldName: nameof(BirthDate));
        }

        BirthDate = birthDate;
        return Result.Success();
    }

    public bool HasEmail(string? email) => string.Equals(TrimToNull(email), Email, StringComparison.OrdinalIgnoreCase);

    public void LinkAccount(Guid accountId) => AccountId = accountId;

    public void CompleteWith(User previousPerson)
    {
        PhoneNumber ??= previousPerson.PhoneNumber;
        BirthDate ??= previousPerson.BirthDate;
    }

    public void Delete(DateTimeOffset deletedAt)
    {
        DeletedAt = SoftDeleteGuard.Delete(DeletedAt, deletedAt);
        IsDeleted = true;
    }

    private static bool IsValidEmail(string email) =>
        email.Length <= EmailMaxLength
        && MailAddress.TryCreate(email, out var mailAddress)
        && string.Equals(mailAddress.Address, email, StringComparison.OrdinalIgnoreCase);

    private static string? TrimToNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
