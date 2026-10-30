namespace ClassManager.Core.Domain.Subscriptions;

public static class Features
{
    public const string Students = "students";
    public const string Branches = "branches";
    public const string Team = "team";
    public const string ClassPacks = "class-packs";
    public const string ImportExport = "import-export";
    public const string CustomRoles = "custom-roles";
    public const string StudentApp = "student-app";
    public const string Shop = "shop";
    public const string Brand = "brand";
    public const string DocumentStorage = "document-storage";
    public const string SelfServiceBilling = "self-service-billing";

    public static IReadOnlyList<string> All { get; } =
    [
        Students,
        Branches,
        Team,
        ClassPacks,
        ImportExport,
        CustomRoles,
        StudentApp,
        Shop,
        Brand,
        DocumentStorage,
        SelfServiceBilling,
    ];

    public static IReadOnlyList<string> Counted { get; } = [Students, Branches, Team, DocumentStorage];
}
