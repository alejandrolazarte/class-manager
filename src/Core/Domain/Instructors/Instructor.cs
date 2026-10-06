using System.Net.Mail;
using ClassManager.Core.Common;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Instructors;

public sealed class Instructor : ITenantOwned
{
    public const int FullNameMinLength = 2;
    public const int FullNameMaxLength = 120;
    public const int EmailMaxLength = 254;

    private const string FullNameLengthMessage = "Full name must be between 2 and 120 characters.";
    private const string EmailFormatMessage = "Email must be a valid address of at most 254 characters.";

    private Instructor()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public bool IsActive { get; private set; }

    public static Result<Instructor> Create(string? fullName, string? email = null)
    {
        var details = ValidDetails(fullName, email);
        if (details.IsFailure)
        {
            return details.Error!;
        }

        return new Instructor
        {
            Id = Guid.CreateVersion7(),
            FullName = details.Value!.FullName,
            Email = details.Value.Email,
            IsActive = true,
        };
    }

    public Result Update(string? fullName, string? email)
    {
        var details = ValidDetails(fullName, email);
        if (details.IsFailure)
        {
            return Result.Failure(details.Error!);
        }

        FullName = details.Value!.FullName;
        Email = details.Value.Email;
        return Result.Success();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private static Result<ValidatedDetails> ValidDetails(string? fullName, string? email)
    {
        var trimmedFullName = fullName?.Trim() ?? string.Empty;
        if (!IsValidFullName(trimmedFullName))
        {
            return Result.Validation<ValidatedDetails>(FullNameLengthMessage, fieldName: nameof(FullName));
        }

        var trimmedEmail = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        if (trimmedEmail is not null && !IsValidEmail(trimmedEmail))
        {
            return Result.Validation<ValidatedDetails>(EmailFormatMessage, fieldName: nameof(Email));
        }

        return new ValidatedDetails(trimmedFullName, trimmedEmail);
    }

    private static bool IsValidFullName(string trimmedFullName) =>
        trimmedFullName.Length is >= FullNameMinLength and <= FullNameMaxLength;

    private static bool IsValidEmail(string email) =>
        email.Length <= EmailMaxLength
        && MailAddress.TryCreate(email, out var mailAddress)
        && string.Equals(mailAddress.Address, email, StringComparison.OrdinalIgnoreCase);

    private sealed record ValidatedDetails(string FullName, string? Email);
}
