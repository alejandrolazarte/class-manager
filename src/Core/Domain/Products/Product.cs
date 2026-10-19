using ClassManager.Core.Common;
using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Images;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Products;

public sealed class Product : ITenantOwned, IAuditable, IHasCatalogImages
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 60;
    public const int DescriptionMaxLength = 500;
    public const int MaximumSizeCount = 20;
    public const int MaximumColorCount = 10;
    public const int MaximumVariantCount = MaximumSizeCount * MaximumColorCount;
    public const int MaximumImages = 5;
    public const string VariantsFieldName = "Variants";

    private const string NameLengthMessage = "The name must be between 2 and 60 characters.";
    private const string DescriptionLengthMessage = "The description must be at most 500 characters.";
    private const string StockModeRequiredMessage = "Choose how stock is kept.";
    private const string VariantCountMessage = "A product needs at least one variant, up to 20 sizes and 10 colors.";
    private const string VariantNameMessage = "Each size needs a name of at most 30 characters.";
    private const string VariantNamesRepeatedMessage = "Two variants have the same size and color.";
    private const string VariantColorMessage = "Each color must be one of the palette, and every variant needs one when any has it.";
    private const string VariantCombinationsMessage = "The variants must be every combination of the sizes and colors.";
    private const string SaleNameSeparator = " · ";
    private const string VariantUnknownMessage = "A size or variant does not belong to this product.";

    private readonly List<ProductVariant> _variants = [];
    private readonly List<ProductImage> _images = [];

    private Product()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public StockMode StockMode { get; private set; }
    public bool IsVisibleInApp { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }
    public IReadOnlyList<ProductImage> Images => _images;
    public IReadOnlyList<Document> ImagesInOrder => CatalogImageGallery.InOrder(_images);
    public int ImageCount => _images.Count;
    public int MaximumImageCount => MaximumImages;
    public IReadOnlyList<ProductVariant> Variants => _variants;

    public bool TracksStock => StockMode != StockMode.Unlimited;

    public static Result<Product> Create(
        string? name,
        string? description,
        decimal? price,
        StockMode? stockMode,
        bool isVisibleInApp,
        IReadOnlyList<VariantChange>? variants,
        DateTimeOffset createdAt)
    {
        var product = new Product
        {
            Id = Guid.CreateVersion7(),
            IsActive = true,
            CreatedAt = createdAt.ToUniversalTime(),
        };
        var update = product.Update(name, description, price, stockMode, isVisibleInApp, variants);
        return update.IsFailure ? update.Error! : product;
    }

    public Result Update(
        string? name,
        string? description,
        decimal? price,
        StockMode? stockMode,
        bool isVisibleInApp,
        IReadOnlyList<VariantChange>? variants)
    {
        var trimmedName = name?.Trim() ?? string.Empty;
        if (trimmedName.Length is < NameMinLength or > NameMaxLength)
        {
            return Result.Validation(NameLengthMessage, fieldName: nameof(Name));
        }

        var trimmedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (trimmedDescription?.Length > DescriptionMaxLength)
        {
            return Result.Validation(DescriptionLengthMessage, fieldName: nameof(Description));
        }

        var priceValidation = MonthlyFee.Validate(price ?? 0, nameof(Price));
        if (priceValidation.IsFailure)
        {
            return priceValidation;
        }

        if (stockMode is null || !Enum.IsDefined(stockMode.Value))
        {
            return Result.Validation(StockModeRequiredMessage, fieldName: nameof(StockMode));
        }

        var variantValidation = ValidateVariants(variants);
        if (variantValidation.IsFailure)
        {
            return variantValidation;
        }

        Name = trimmedName;
        Description = trimmedDescription;
        Price = price!.Value;
        StockMode = stockMode.Value;
        IsVisibleInApp = isVisibleInApp;
        ApplyVariants(variants!);
        return Result.Success();
    }

    public ProductVariant? FindVariant(Guid variantId) => _variants.FirstOrDefault(variant => variant.Id == variantId);

    public string SaleNameOf(ProductVariant variant) =>
        variant.Label.Length == 0 ? Name : $"{Name}{SaleNameSeparator}{variant.Label}";

    public void AddImage(Document document) => _images.Add(ProductImage.Create(Id, document, _images.Count));

    public Document? RemoveImage(Guid documentId) => CatalogImageGallery.Remove(_images, documentId);

    public Result ReorderImages(IReadOnlyList<Guid>? documentIds) => CatalogImageGallery.Reorder(_images, documentIds);

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private static string SizeOf(VariantChange variant) => variant.Size?.Trim() ?? string.Empty;

    private static string? ColorOf(VariantChange variant) => string.IsNullOrWhiteSpace(variant.Color) ? null : variant.Color.Trim();

    private Result ValidateVariants(IReadOnlyList<VariantChange>? variants)
    {
        if (variants is null || variants.Count is 0 or > MaximumVariantCount)
        {
            return Result.Validation(VariantCountMessage, fieldName: VariantsFieldName);
        }

        var sizes = variants.Select(SizeOf).ToList();
        var colors = variants.Select(ColorOf).ToList();
        var hasColors = colors.Any(color => color is not null);
        if (hasColors && colors.Any(color => color is null || !ProductColors.IsKnown(color)))
        {
            return Result.Validation(VariantColorMessage, fieldName: VariantsFieldName);
        }

        var hasSizes = sizes.Any(size => size.Length > 0);
        var needsSizes = hasSizes || (!hasColors && variants.Count > 1);
        if (sizes.Any(size => size.Length > ProductVariant.SizeMaxLength || (needsSizes && size.Length == 0)))
        {
            return Result.Validation(VariantNameMessage, fieldName: VariantsFieldName);
        }

        var distinctSizeCount = sizes.Distinct(StringComparer.OrdinalIgnoreCase).Count();
        var distinctColorCount = colors.Distinct().Count();
        if (distinctSizeCount > MaximumSizeCount || distinctColorCount > MaximumColorCount)
        {
            return Result.Validation(VariantCountMessage, fieldName: VariantsFieldName);
        }

        var combinations = variants.Select(variant => $"{SizeOf(variant).ToUpperInvariant()}|{ColorOf(variant)}").ToList();
        if (combinations.Distinct().Count() != combinations.Count)
        {
            return Result.Validation(VariantNamesRepeatedMessage, fieldName: VariantsFieldName);
        }

        if (distinctSizeCount * distinctColorCount != variants.Count)
        {
            return Result.Validation(VariantCombinationsMessage, fieldName: VariantsFieldName);
        }

        var variantIds = variants.Where(variant => variant.Id is not null).Select(variant => variant.Id!.Value).ToList();
        if (variantIds.Distinct().Count() != variantIds.Count || variantIds.Any(variantId => FindVariant(variantId) is null))
        {
            return Result.Validation(VariantUnknownMessage, fieldName: VariantsFieldName);
        }

        return Result.Success();
    }

    private void ApplyVariants(IReadOnlyList<VariantChange> variants)
    {
        var keptVariantIds = variants.Where(variant => variant.Id is not null).Select(variant => variant.Id!.Value).ToHashSet();
        foreach (var removedVariant in _variants.Where(variant => !keptVariantIds.Contains(variant.Id)))
        {
            removedVariant.Deactivate();
        }

        for (var position = 0; position < variants.Count; position++)
        {
            var change = variants[position];
            if (change.Id is { } variantId)
            {
                FindVariant(variantId)!.Change(SizeOf(change), ColorOf(change), position);
                continue;
            }

            _variants.Add(ProductVariant.Create(Id, SizeOf(change), ColorOf(change), position));
        }
    }
}
