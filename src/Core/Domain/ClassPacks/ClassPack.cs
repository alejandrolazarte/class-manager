using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Images;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.ClassPacks;

public sealed class ClassPack : ITenantOwned, IAuditable, IHasCatalogImages
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 60;
    public const int MinimumClassCount = 1;
    public const int MaximumClassCount = 100;
    public const int MinimumValidityMonths = 1;
    public const int MaximumValidityMonths = 24;
    public const int DescriptionMaxLength = 500;
    public const int MaximumImages = 1;

    private const string NameLengthMessage = "The name must be between 2 and 60 characters.";
    private const string ClassCountRangeMessage = "The number of classes must be between 1 and 100.";
    private const string ValidityRangeMessage = "The validity must be between 1 and 24 months.";
    private const string ClassDurationMessage = "Class duration must be between 15 and 240 minutes, in steps of 5.";
    private const string PrivateLessonDurationRequiredMessage = "A pack for private lessons needs the lesson duration.";
    private const string GroupClassDurationNotAllowedMessage = "A pack for group classes uses the duration of each class.";
    private const string DescriptionLengthMessage = "The description must be at most 500 characters.";
    private const string RenewingValidityMessage = "A plan that renews every month lasts a month; leave the validity empty.";

    private readonly List<ClassPackClassGroup> _classGroups = [];
    private readonly List<ClassPackImage> _images = [];

    private ClassPack()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int? ClassCount { get; private set; }
    public bool RenewsMonthly { get; private set; }
    public bool AppliesFamilyDiscount { get; private set; }
    public decimal Price { get; private set; }
    public int? ValidityMonths { get; private set; }
    public int? ClassDurationMinutes { get; private set; }
    public bool CoversAnyGroupClass { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public IReadOnlyList<ClassPackImage> Images => _images;
    public IReadOnlyList<Document> ImagesInOrder => CatalogImageGallery.InOrder(_images);
    public int ImageCount => _images.Count;
    public int MaximumImageCount => MaximumImages;
    public IReadOnlyList<ClassPackClassGroup> ClassGroups => _classGroups;
    public IReadOnlyList<long> ClassGroupIds => [.. _classGroups.Select(classGroup => classGroup.ClassGroupId)];

    public bool IsSoldOnce => !RenewsMonthly;

    public static Result<ClassPack> Create(
        string? name,
        int? classCount,
        decimal? price,
        int? validityMonths,
        DateTimeOffset createdAt,
        bool renewsMonthly = false,
        bool? appliesFamilyDiscount = null)
    {
        var pack = new ClassPack
        {
            IsActive = true,
            CreatedAt = createdAt.ToUniversalTime(),
        };
        var update = pack.Update(name, classCount, price, validityMonths, renewsMonthly, appliesFamilyDiscount);
        return update.IsFailure ? update.Error! : pack;
    }

    public Result Update(
        string? name,
        int? classCount,
        decimal? price,
        int? validityMonths,
        bool renewsMonthly = false,
        bool? appliesFamilyDiscount = null)
    {
        var trimmedName = name?.Trim() ?? string.Empty;
        if (trimmedName.Length is < NameMinLength or > NameMaxLength)
        {
            return Result.Validation(NameLengthMessage, fieldName: nameof(Name));
        }

        var hasLibreClasses = renewsMonthly && classCount is null;
        if (!hasLibreClasses && classCount is null or < MinimumClassCount or > MaximumClassCount)
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

        if (renewsMonthly && validityMonths is not null)
        {
            return Result.Validation(RenewingValidityMessage, fieldName: nameof(ValidityMonths));
        }

        Name = trimmedName;
        ClassCount = classCount;
        Price = price!.Value;
        ValidityMonths = validityMonths;
        RenewsMonthly = renewsMonthly;
        AppliesFamilyDiscount = appliesFamilyDiscount ?? renewsMonthly;
        return Result.Success();
    }

    public Result DefineLessons(IReadOnlyCollection<long> classGroupIds, int? classDurationMinutes, bool coversAnyGroupClass = false)
    {
        if (classDurationMinutes is not null
            && (classDurationMinutes is < ClassSchedule.MinimumDurationMinutes or > ClassSchedule.MaximumDurationMinutes
                || classDurationMinutes % ClassSchedule.DurationStepMinutes != 0))
        {
            return Result.Validation(ClassDurationMessage, fieldName: nameof(ClassDurationMinutes));
        }

        if (coversAnyGroupClass)
        {
            if (classDurationMinutes is not null)
            {
                return Result.Validation(GroupClassDurationNotAllowedMessage, fieldName: nameof(ClassDurationMinutes));
            }

            ClassDurationMinutes = null;
            CoversAnyGroupClass = true;
            CoverClassGroups([]);
            return Result.Success();
        }

        if (classGroupIds.Count == 0 && classDurationMinutes is null)
        {
            return Result.Validation(PrivateLessonDurationRequiredMessage, fieldName: nameof(ClassDurationMinutes));
        }

        if (classGroupIds.Count > 0 && classDurationMinutes is not null)
        {
            return Result.Validation(GroupClassDurationNotAllowedMessage, fieldName: nameof(ClassDurationMinutes));
        }

        ClassDurationMinutes = classDurationMinutes;
        CoversAnyGroupClass = false;
        CoverClassGroups(classGroupIds);
        return Result.Success();
    }

    public Result Describe(string? description)
    {
        var trimmedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (trimmedDescription?.Length > DescriptionMaxLength)
        {
            return Result.Validation(DescriptionLengthMessage, fieldName: nameof(Description));
        }

        Description = trimmedDescription;
        return Result.Success();
    }

    private void CoverClassGroups(IEnumerable<long> classGroupIds)
    {
        var requestedIds = classGroupIds.ToHashSet();
        _classGroups.RemoveAll(classGroup => !requestedIds.Contains(classGroup.ClassGroupId));
        foreach (var classGroupId in requestedIds.Where(classGroupId => !CoversClassGroup(classGroupId)))
        {
            _classGroups.Add(ClassPackClassGroup.Create(Id, classGroupId));
        }
    }

    private bool CoversClassGroup(long classGroupId) => _classGroups.Any(classGroup => classGroup.ClassGroupId == classGroupId);

    public void AddImage(Document document) => _images.Add(ClassPackImage.Create(Id, document, _images.Count));

    public Document? RemoveImage(long documentId) => CatalogImageGallery.Remove(_images, documentId);

    public Result ReorderImages(IReadOnlyList<long>? documentIds) => CatalogImageGallery.Reorder(_images, documentIds);

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
