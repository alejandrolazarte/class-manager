using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Images;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.ClassPacks;

public sealed class ClassPack : ITenantOwned, IHasCatalogImages
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 60;
    public const int MinimumClassCount = 1;
    public const int MaximumClassCount = 100;
    public const int MinimumValidityMonths = 1;
    public const int MaximumValidityMonths = 24;
    public const int MaterialUrlMaxLength = 500;
    public const int DescriptionMaxLength = 500;

    private const string NameLengthMessage = "The name must be between 2 and 60 characters.";
    private const string ClassCountRangeMessage = "The number of classes must be between 1 and 100.";
    private const string ValidityRangeMessage = "The validity must be between 1 and 24 months.";
    private const string ClassDurationMessage = "Class duration must be between 15 and 240 minutes, in steps of 5.";
    private const string MaterialUrlMessage = "The material link must be an https address of at most 500 characters.";
    private const string DescriptionLengthMessage = "The description must be at most 500 characters.";

    private readonly List<ClassPackClassGroup> _classGroups = [];
    private readonly List<ClassPackImage> _images = [];

    private ClassPack()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int ClassCount { get; private set; }
    public decimal Price { get; private set; }
    public int? ValidityMonths { get; private set; }
    public int? ClassDurationMinutes { get; private set; }
    public string? MaterialUrl { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public IReadOnlyList<ClassPackImage> Images => _images;
    public IReadOnlyList<Document> ImagesInOrder => CatalogImageGallery.InOrder(_images);
    public int ImageCount => _images.Count;
    public IReadOnlyList<ClassPackClassGroup> ClassGroups => _classGroups;
    public IReadOnlyList<Guid> ClassGroupIds => [.. _classGroups.Select(classGroup => classGroup.ClassGroupId)];

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

    public Result DefineLessons(int? classDurationMinutes, string? materialUrl)
    {
        if (classDurationMinutes is not null
            && (classDurationMinutes is < ClassSchedule.MinimumDurationMinutes or > ClassSchedule.MaximumDurationMinutes
                || classDurationMinutes % ClassSchedule.DurationStepMinutes != 0))
        {
            return Result.Validation(ClassDurationMessage, fieldName: nameof(ClassDurationMinutes));
        }

        var trimmedUrl = string.IsNullOrWhiteSpace(materialUrl) ? null : materialUrl.Trim();
        if (trimmedUrl is not null
            && (trimmedUrl.Length > MaterialUrlMaxLength
                || !Uri.TryCreate(trimmedUrl, UriKind.Absolute, out var parsedUrl)
                || parsedUrl.Scheme != Uri.UriSchemeHttps))
        {
            return Result.Validation(MaterialUrlMessage, fieldName: nameof(MaterialUrl));
        }

        ClassDurationMinutes = classDurationMinutes;
        MaterialUrl = trimmedUrl;
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

    public void CoverClassGroups(IEnumerable<Guid> classGroupIds)
    {
        var requestedIds = classGroupIds.ToHashSet();
        _classGroups.RemoveAll(classGroup => !requestedIds.Contains(classGroup.ClassGroupId));
        foreach (var classGroupId in requestedIds.Where(classGroupId => !CoversClassGroup(classGroupId)))
        {
            _classGroups.Add(ClassPackClassGroup.Create(Id, classGroupId));
        }
    }

    public bool CoversClassGroup(Guid classGroupId) => _classGroups.Any(classGroup => classGroup.ClassGroupId == classGroupId);

    public void AddImage(Document document) => _images.Add(ClassPackImage.Create(Id, document, _images.Count));

    public Document? RemoveImage(Guid documentId) => CatalogImageGallery.Remove(_images, documentId);

    public Result ReorderImages(IReadOnlyList<Guid>? documentIds) => CatalogImageGallery.Reorder(_images, documentIds);

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
