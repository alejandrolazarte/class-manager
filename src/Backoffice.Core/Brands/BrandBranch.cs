namespace ClassManager.Backoffice.Core.Brands;

public sealed record BrandBranch(
    long Id,
    string Name,
    string CountryCode,
    DateTimeOffset CreatedAt,
    int StudentCount,
    int TeamCount,
    long DocumentBytes);
