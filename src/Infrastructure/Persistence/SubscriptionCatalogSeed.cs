using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Catalog;

namespace ClassManager.Infrastructure.Persistence;

internal static class SubscriptionCatalogSeed
{
    public const string ListPriceCurrency = "EUR";

    private const decimal LitePrice = 19m;
    private const decimal ProPrice = 39m;
    private const decimal ExtraBranchPrice = 20m;
    private const decimal ExtraDocumentStoragePrice = 2m;

    private const int FreeTrialDays = 30;
    private const int FreeStudents = 30;
    private const int LiteStudents = 100;
    private const int ProStudents = 300;
    private const int SingleBranch = 1;
    private const int EnterpriseBranches = 3;
    private const int FreeTeam = 0;
    private const int LiteTeam = 2;
    private const int ProTeam = 10;
    private const int FreeDocumentStorageInMegabytes = 100;
    private const int LiteDocumentStorageInMegabytes = 1024;
    private const int ProDocumentStorageInMegabytes = 5 * 1024;
    private const int EnterpriseDocumentStorageInMegabytes = 10 * 1024;

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
            Feature.Create(Features.ImportExport, isCounted: false),
            Feature.Create(Features.CustomRoles, isCounted: false),
            Feature.Create(Features.StudentApp, isCounted: false),
            Feature.Create(Features.Shop, isCounted: false),
            Feature.Create(Features.Brand, isCounted: false),
            Feature.CreateAddOn(Features.DocumentStorage, isCounted: true, ExtraDocumentStoragePrice, ListPriceCurrency));

        modelBuilder.Entity<PlanFeature>().HasData(
            PlanFeature.Create(PlanCodes.Free, Features.Students, FreeStudents),
            PlanFeature.Create(PlanCodes.Free, Features.Branches, SingleBranch),
            PlanFeature.Create(PlanCodes.Free, Features.Team, FreeTeam),
            PlanFeature.Create(PlanCodes.Free, Features.ImportExport),
            PlanFeature.Create(PlanCodes.Free, Features.StudentApp),
            PlanFeature.Create(PlanCodes.Free, Features.DocumentStorage, FreeDocumentStorageInMegabytes),
            PlanFeature.Create(PlanCodes.Lite, Features.Students, LiteStudents),
            PlanFeature.Create(PlanCodes.Lite, Features.Branches, SingleBranch),
            PlanFeature.Create(PlanCodes.Lite, Features.Team, LiteTeam),
            PlanFeature.Create(PlanCodes.Lite, Features.ClassPacks),
            PlanFeature.Create(PlanCodes.Lite, Features.ImportExport),
            PlanFeature.Create(PlanCodes.Lite, Features.StudentApp),
            PlanFeature.Create(PlanCodes.Lite, Features.DocumentStorage, LiteDocumentStorageInMegabytes),
            PlanFeature.Create(PlanCodes.Pro, Features.Students, ProStudents),
            PlanFeature.Create(PlanCodes.Pro, Features.Branches, SingleBranch),
            PlanFeature.Create(PlanCodes.Pro, Features.Team, ProTeam),
            PlanFeature.Create(PlanCodes.Pro, Features.ClassPacks),
            PlanFeature.Create(PlanCodes.Pro, Features.ImportExport),
            PlanFeature.Create(PlanCodes.Pro, Features.CustomRoles),
            PlanFeature.Create(PlanCodes.Pro, Features.StudentApp),
            PlanFeature.Create(PlanCodes.Pro, Features.Shop),
            PlanFeature.Create(PlanCodes.Pro, Features.Brand),
            PlanFeature.Create(PlanCodes.Pro, Features.DocumentStorage, ProDocumentStorageInMegabytes),
            PlanFeature.Create(PlanCodes.Enterprise, Features.Students),
            PlanFeature.Create(PlanCodes.Enterprise, Features.Branches, EnterpriseBranches),
            PlanFeature.Create(PlanCodes.Enterprise, Features.Team),
            PlanFeature.Create(PlanCodes.Enterprise, Features.ClassPacks),
            PlanFeature.Create(PlanCodes.Enterprise, Features.ImportExport),
            PlanFeature.Create(PlanCodes.Enterprise, Features.CustomRoles),
            PlanFeature.Create(PlanCodes.Enterprise, Features.StudentApp),
            PlanFeature.Create(PlanCodes.Enterprise, Features.Shop),
            PlanFeature.Create(PlanCodes.Enterprise, Features.Brand),
            PlanFeature.Create(PlanCodes.Enterprise, Features.DocumentStorage, EnterpriseDocumentStorageInMegabytes));

        return modelBuilder;
    }
}
