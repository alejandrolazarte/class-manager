namespace ClassManager.Backoffice.Core.Brands;

public sealed record BrandBranch(
    Guid Id,
    string Name,
    string CountryCode,
    DateTimeOffset CreatedAt,
    int StudentCount,
    int TeamCount,
    long DocumentBytes);
