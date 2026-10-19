using ClassManager.Core.Audit;

namespace ClassManager.Core.Domain.Products;

public sealed record VariantChange(Guid? Id, [property: AuditPlain] string? Size, [property: AuditPlain] string? Color = null, int? InitialStock = null);
