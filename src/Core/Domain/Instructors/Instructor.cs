using ClassManager.Core.Common;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Instructors;

public sealed class Instructor : ITenantOwned
{
    public const int FullNameMinLength = 2;
    public const int FullNameMaxLength = 120;

    private const string FullNameLengthMessage = "Full name must be between 2 and 120 characters.";

    private Instructor()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    public static Result<Instructor> Create(string? fullName)
    {
        var trimmedFullName = fullName?.Trim() ?? string.Empty;
        if (!IsValidFullName(trimmedFullName))
        {
            return Result.Validation<Instructor>(FullNameLengthMessage, fieldName: nameof(FullName));
        }

        return new Instructor
        {
            Id = Guid.CreateVersion7(),
            FullName = trimmedFullName,
            IsActive = true,
        };
    }

    public Result Rename(string? fullName)
    {
        var trimmedFullName = fullName?.Trim() ?? string.Empty;
        if (!IsValidFullName(trimmedFullName))
        {
            return Result.Validation(FullNameLengthMessage, fieldName: nameof(FullName));
        }

        FullName = trimmedFullName;
        return Result.Success();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private static bool IsValidFullName(string trimmedFullName) =>
        trimmedFullName.Length is >= FullNameMinLength and <= FullNameMaxLength;
}
