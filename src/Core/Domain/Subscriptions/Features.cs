namespace ClassManager.Core.Domain.Subscriptions;

public static class Features
{
    public const string Students = "students";
    public const string Branches = "branches";
    public const string Team = "team";
    public const string ClassPacks = "class-packs";
    public const string ImportExport = "import-export";
    public const string CustomRoles = "custom-roles";
    public const string FamilyApp = "family-app";
    public const string Shop = "shop";
    public const string Brand = "brand";
    public const string CatalogPhotos = "catalog-photos";

    public static IReadOnlyList<string> All { get; } =
    [
        Students,
        Branches,
        Team,
        ClassPacks,
        ImportExport,
        CustomRoles,
        FamilyApp,
        Shop,
        Brand,
        CatalogPhotos,
    ];

    public static IReadOnlyList<string> Counted { get; } = [Students, Branches, Team, CatalogPhotos];
}
