namespace ClassManager.Core.Domain.Products;

public sealed record VariantChange(Guid? Id, string? Size, string? Color = null, int? InitialStock = null);
