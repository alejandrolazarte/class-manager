using System.Net.Mail;

using ClassManager.Core.Common;

namespace ClassManager.Core.Domain.Accounts;

public sealed class OwnerProfile
{
    private const string FullNameLengthMessage = "Full name must be between 2 and 120 characters.";
    private const string EmailFormatMessage = "Email must be a valid address of at most 254 characters.";
    private const string BirthDateMessage = "Birth date is required and must be between 1900-01-01 and today.";

    private OwnerProfile(string fullName, string email, DateOnly birthDate)
    {
        FullName = fullName;
        Email = email;
        BirthDate = birthDate;
    }

    public string FullName { get; }
    public string Email { get; }
    public DateOnly BirthDate { get; }

    public static Result<OwnerProfile> Create(string? fullName, string? email, DateOnly? birthDate, DateOnly today)
    {
        var trimmedFullName = Trimmed(fullName);
        var trimmedEmail = Trimmed(email);
        if (FullNameOrEmailError(trimmedFullName, trimmedEmail) is { } fullNameOrEmailError)
        {
            return fullNameOrEmailError;
        }

        if (birthDate is not { } validBirthDate || !PersonAge.IsValidBirthDate(validBirthDate, today))
        {
            return Result.Validation<OwnerProfile>(BirthDateMessage, fieldName: nameof(BirthDate));
        }

        return new OwnerProfile(trimmedFullName, trimmedEmail, validBirthDate);
    }

    internal static string Trimmed(string? text) => text?.Trim() ?? string.Empty;

    internal static ResultError? FullNameOrEmailError(string trimmedFullName, string trimmedEmail)
    {
        if (trimmedFullName.Length is < OwnerAccount.FullNameMinLength or > OwnerAccount.FullNameMaxLength)
        {
            return Result.Validation<OwnerProfile>(FullNameLengthMessage, fieldName: nameof(FullName)).Error;
        }

        return IsValidEmail(trimmedEmail)
            ? null
            : Result.Validation<OwnerProfile>(EmailFormatMessage, fieldName: nameof(Email)).Error;
    }

    private static bool IsValidEmail(string email) =>
        email.Length is > 0 and <= OwnerAccount.EmailMaxLength
        && MailAddress.TryCreate(email, out var mailAddress)
        && string.Equals(mailAddress.Address, email, StringComparison.OrdinalIgnoreCase);
}
