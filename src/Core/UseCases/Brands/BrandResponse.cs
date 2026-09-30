using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Brands;

public sealed record BrandResponse(
    string DisplayName,
    string? BrandName,
    string? ThemeColor,
    string? AccentColor,
    bool LocksTheme,
    DateTimeOffset? LogoUpdatedAt)
{
    public static BrandResponse From(Business business) =>
        new(
            business.BrandDisplayName,
            business.BrandName,
            business.ThemeColor,
            business.AccentColor,
            business.LocksTheme,
            business.LogoUpdatedAt);
}
