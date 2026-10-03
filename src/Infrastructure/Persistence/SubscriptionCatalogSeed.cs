using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Catalog;

namespace ClassManager.Infrastructure.Persistence;

internal static class SubscriptionCatalogSeed
{
    public const string ListPriceCurrency = "USD";

    private const decimal LitePrice = 9m;
    private const decimal ProPrice = 19m;
    private const decimal ExtraBranchPrice = 10m;
    private const decimal ImportExportPrice = 3m;
    private const decimal FamilyAppPrice = 5m;
    private const decimal ShopPrice = 5m;
    private const decimal BrandPrice = 5m;
    private const decimal ExtraCatalogPhotosPrice = 3m;

    private const int FreeTrialDays = 30;
    private const int FreeStudents = 30;
    private const int LiteStudents = 150;
    private const int SingleBranch = 1;
    private const int ProBranches = 3;
    private const int FreeTeam = 0;
    private const int LiteTeam = 2;
    private const int ProTeam = 10;
    private const int FreeCatalogPhotos = 1;
    private const int PaidCatalogPhotos = 5;

    public static ModelBuilder SeedSubscriptionCatalog(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Plan>().HasData(
            Plan.Create(PlanCodes.Free, displayOrder: 1, 0m, ListPriceCurrency, BillingPeriod.Monthly, isDefault: true, FreeTrialDays),
            Plan.Create(PlanCodes.Lite, displayOrder: 2, LitePrice, ListPriceCurrency, BillingPeriod.Monthly),
            Plan.Create(PlanCodes.Pro, displayOrder: 3, ProPrice, ListPriceCurrency, BillingPeriod.Monthly),
            Plan.Create(PlanCodes.Enterprise, displayOrder: 4, listPrice: null, ListPriceCurrency, BillingPeriod.Monthly));

        modelBuilder.Entity<Feature>().HasData(
            Feature.Create(Features.Students, isCounted: true),
            Feature.CreateAddOn(Features.Branches, isCounted: true, ExtraBranchPrice, ListPriceCurrency),
            Feature.Create(Features.Team, isCounted: true),
            Feature.Create(Features.ClassPacks, isCounted: false),
            Feature.CreateAddOn(Features.ImportExport, isCounted: false, ImportExportPrice, ListPriceCurrency),
            Feature.Create(Features.CustomRoles, isCounted: false),
            Feature.CreateAddOn(Features.FamilyApp, isCounted: false, FamilyAppPrice, ListPriceCurrency),
            Feature.CreateAddOn(Features.Shop, isCounted: false, ShopPrice, ListPriceCurrency),
            Feature.CreateAddOn(Features.Brand, isCounted: false, BrandPrice, ListPriceCurrency),
            Feature.CreateAddOn(Features.CatalogPhotos, isCounted: true, ExtraCatalogPhotosPrice, ListPriceCurrency));

        modelBuilder.Entity<PlanFeature>().HasData(
            PlanFeature.Create(PlanCodes.Free, Features.Students, FreeStudents),
            PlanFeature.Create(PlanCodes.Free, Features.Branches, SingleBranch),
            PlanFeature.Create(PlanCodes.Free, Features.Team, FreeTeam),
            PlanFeature.Create(PlanCodes.Free, Features.CatalogPhotos, FreeCatalogPhotos),
            PlanFeature.Create(PlanCodes.Lite, Features.Students, LiteStudents),
            PlanFeature.Create(PlanCodes.Lite, Features.Branches, SingleBranch),
            PlanFeature.Create(PlanCodes.Lite, Features.Team, LiteTeam),
            PlanFeature.Create(PlanCodes.Lite, Features.ClassPacks),
            PlanFeature.Create(PlanCodes.Lite, Features.ImportExport),
            PlanFeature.Create(PlanCodes.Lite, Features.CatalogPhotos, PaidCatalogPhotos),
            PlanFeature.Create(PlanCodes.Pro, Features.Students),
            PlanFeature.Create(PlanCodes.Pro, Features.Branches, ProBranches),
            PlanFeature.Create(PlanCodes.Pro, Features.Team, ProTeam),
            PlanFeature.Create(PlanCodes.Pro, Features.ClassPacks),
            PlanFeature.Create(PlanCodes.Pro, Features.ImportExport),
            PlanFeature.Create(PlanCodes.Pro, Features.CustomRoles),
            PlanFeature.Create(PlanCodes.Pro, Features.FamilyApp),
            PlanFeature.Create(PlanCodes.Pro, Features.Shop),
            PlanFeature.Create(PlanCodes.Pro, Features.Brand),
            PlanFeature.Create(PlanCodes.Pro, Features.CatalogPhotos, PaidCatalogPhotos),
            PlanFeature.Create(PlanCodes.Enterprise, Features.Students),
            PlanFeature.Create(PlanCodes.Enterprise, Features.Branches),
            PlanFeature.Create(PlanCodes.Enterprise, Features.Team),
            PlanFeature.Create(PlanCodes.Enterprise, Features.ClassPacks),
            PlanFeature.Create(PlanCodes.Enterprise, Features.ImportExport),
            PlanFeature.Create(PlanCodes.Enterprise, Features.CustomRoles),
            PlanFeature.Create(PlanCodes.Enterprise, Features.FamilyApp),
            PlanFeature.Create(PlanCodes.Enterprise, Features.Shop),
            PlanFeature.Create(PlanCodes.Enterprise, Features.Brand),
            PlanFeature.Create(PlanCodes.Enterprise, Features.CatalogPhotos, PaidCatalogPhotos));

        return modelBuilder;
    }
}
