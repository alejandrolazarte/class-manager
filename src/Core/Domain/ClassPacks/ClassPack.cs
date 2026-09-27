using ClassManager.Core.Common;
using ClassManager.Core.Domain.Fees;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.ClassPacks;

public sealed class ClassPack : ITenantOwned
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 60;
    public const int MinimumClassCount = 1;
    public const int MaximumClassCount = 100;
    public const int MinimumValidityMonths = 1;
    public const int MaximumValidityMonths = 24;

    private const string NameLengthMessage = "The name must be between 2 and 60 characters.";
    private const string ClassCountRangeMessage = "The number of classes must be between 1 and 100.";
    private const string ValidityRangeMessage = "The validity must be between 1 and 24 months.";

    private ClassPack()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int ClassCount { get; private set; }
    public decimal Price { get; private set; }
    public int? ValidityMonths { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static Result<ClassPack> Create(string? name, int? classCount, decimal? price, int? validityMonths, DateTimeOffset createdAt)
    {
        var pack = new ClassPack
        {
            Id = Guid.CreateVersion7(),
            IsActive = true,
            CreatedAt = createdAt.ToUniversalTime(),
        };
        var update = pack.Update(name, classCount, price, validityMonths);
        return update.IsFailure ? update.Error! : pack;
    }

    public Result Update(string? name, int? classCount, decimal? price, int? validityMonths)
    {
        var trimmedName = name?.Trim() ?? string.Empty;
        if (trimmedName.Length is < NameMinLength or > NameMaxLength)
        {
            return Result.Validation(NameLengthMessage, fieldName: nameof(Name));
        }

        if (classCount is null or < MinimumClassCount or > MaximumClassCount)
        {
            return Result.Validation(ClassCountRangeMessage, fieldName: nameof(ClassCount));
        }

        var priceValidation = MonthlyFee.Validate(price ?? 0, nameof(Price));
        if (priceValidation.IsFailure)
        {
            return priceValidation;
        }

        if (validityMonths is < MinimumValidityMonths or > MaximumValidityMonths)
        {
            return Result.Validation(ValidityRangeMessage, fieldName: nameof(ValidityMonths));
        }

        Name = trimmedName;
        ClassCount = classCount.Value;
        Price = price!.Value;
        ValidityMonths = validityMonths;
        return Result.Success();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
