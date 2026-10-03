using ClassManager.Core.Common;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Images;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Products;

public sealed class Product : ITenantOwned, IHasCatalogImage
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 60;
    public const int DescriptionMaxLength = 500;
    public const int MaximumVariantCount = 20;
    public const string VariantsFieldName = "Variants";

    private const string NameLengthMessage = "The name must be between 2 and 60 characters.";
    private const string DescriptionLengthMessage = "The description must be at most 500 characters.";
    private const string StockModeRequiredMessage = "Choose how stock is kept.";
    private const string VariantCountMessage = "A product needs between 1 and 20 sizes or variants.";
    private const string VariantNameMessage = "Each size or variant needs a name of at most 30 characters.";
    private const string VariantNamesRepeatedMessage = "Two sizes or variants have the same name.";
    private const string VariantUnknownMessage = "A size or variant does not belong to this product.";

    private readonly List<ProductVariant> _variants = [];

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
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
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
        string.IsNullOrEmpty(variant.Name) ? Name : $"{Name} · {variant.Name}";

    public void ChangeImage(Uri imageUrl) => ImageUrl = imageUrl.AbsoluteUri;

    public void RemoveImage() => ImageUrl = null;

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private Result ValidateVariants(IReadOnlyList<VariantChange>? variants)
    {
        if (variants is null || variants.Count is 0 or > MaximumVariantCount)
        {
            return Result.Validation(VariantCountMessage, fieldName: VariantsFieldName);
        }

        var names = variants.Select(variant => variant.Name?.Trim() ?? string.Empty).ToList();
        var needsNames = variants.Count > 1;
        if (names.Any(variantName => variantName.Length > ProductVariant.NameMaxLength || (needsNames && variantName.Length == 0)))
        {
            return Result.Validation(VariantNameMessage, fieldName: VariantsFieldName);
        }

        if (names.Distinct(StringComparer.OrdinalIgnoreCase).Count() != names.Count)
        {
            return Result.Validation(VariantNamesRepeatedMessage, fieldName: VariantsFieldName);
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
            var variantName = change.Name?.Trim() ?? string.Empty;
            if (change.Id is { } variantId)
            {
                FindVariant(variantId)!.Rename(variantName, position);
                continue;
            }

            _variants.Add(ProductVariant.Create(Id, variantName, position));
        }
    }
}
