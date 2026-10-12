using ClassManager.Core.Common;

namespace ClassManager.Core.Domain.Accounts;

public sealed class OwnerAccount
{
    public const int FullNameMinLength = 2;
    public const int FullNameMaxLength = 120;
    public const int EmailMaxLength = 254;
    public const int PasswordMinLength = 10;
    public const int PasswordMaxLength = 128;

    private const string PasswordLengthMessage = "Password must be between 10 and 128 characters.";

    private OwnerAccount(OwnerProfile profile, string password)
    {
        FullName = profile.FullName;
        Email = profile.Email;
        Password = password;
        BirthDate = profile.BirthDate;
    }

    public string FullName { get; }
    public string Email { get; }
    public string Password { get; }
    public DateOnly BirthDate { get; }

    public static Result<OwnerAccount> Create(string? fullName, string? email, string? password, DateOnly? birthDate, DateOnly today)
    {
        var profile = OwnerProfile.Create(fullName, email, birthDate, today);
        if (profile.IsFailure)
        {
            return profile.Error!;
        }

        return Create(profile.Value!, password);
    }

    public static Result<OwnerAccount> Create(OwnerProfile profile, string? password)
    {
        ArgumentNullException.ThrowIfNull(profile);
        if (password is null || password.Length is < PasswordMinLength or > PasswordMaxLength)
        {
            return Result.Validation<OwnerAccount>(PasswordLengthMessage, fieldName: nameof(Password));
        }

        return new OwnerAccount(profile, password);
    }
}
