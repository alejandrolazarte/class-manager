using System.Net.Mail;

using ClassManager.Core.Common;

namespace ClassManager.Core.Domain.Accounts;

public sealed class OwnerAccount
{
    public const int FullNameMinLength = 2;
    public const int FullNameMaxLength = 120;
    public const int EmailMaxLength = 254;
    public const int PasswordMinLength = 10;
    public const int PasswordMaxLength = 128;

    private const string FullNameLengthMessage = "Full name must be between 2 and 120 characters.";
    private const string EmailFormatMessage = "Email must be a valid address of at most 254 characters.";
    private const string PasswordLengthMessage = "Password must be between 10 and 128 characters.";

    private OwnerAccount(string fullName, string email, string password)
    {
        FullName = fullName;
        Email = email;
        Password = password;
    }

    public string FullName { get; }
    public string Email { get; }
    public string Password { get; }

    public static Result<OwnerAccount> Create(string? fullName, string? email, string? password)
    {
        var trimmedFullName = fullName?.Trim() ?? string.Empty;
        if (trimmedFullName.Length is < FullNameMinLength or > FullNameMaxLength)
        {
            return Result.Validation<OwnerAccount>(FullNameLengthMessage, fieldName: nameof(FullName));
        }

        var trimmedEmail = email?.Trim() ?? string.Empty;
        if (!IsValidEmail(trimmedEmail))
        {
            return Result.Validation<OwnerAccount>(EmailFormatMessage, fieldName: nameof(Email));
        }

        if (password is null || password.Length is < PasswordMinLength or > PasswordMaxLength)
        {
            return Result.Validation<OwnerAccount>(PasswordLengthMessage, fieldName: nameof(Password));
        }

        return new OwnerAccount(trimmedFullName, trimmedEmail, password);
    }

    private static bool IsValidEmail(string email) =>
        email.Length is > 0 and <= EmailMaxLength
        && MailAddress.TryCreate(email, out var mailAddress)
        && string.Equals(mailAddress.Address, email, StringComparison.OrdinalIgnoreCase);
}
