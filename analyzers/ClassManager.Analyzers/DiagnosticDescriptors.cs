using Microsoft.CodeAnalysis;

namespace ClassManager.Analyzers;

internal static class DiagnosticDescriptors
{
    private const string TestingCategory = "Testing";
    private const string ArchitectureCategory = "Architecture";

    public static readonly DiagnosticDescriptor TestFileNameMustStartWithThen = new(
        id: "TEST001",
        title: "Test file name must start with 'Then_'",
        messageFormat: "Test file '{0}' must be named 'Then_<result>.cs'",
        category: TestingCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor TestFileMustBeInWhenFolder = new(
        id: "TEST002",
        title: "Test file must be inside a 'When_' folder",
        messageFormat: "Test file is in folder '{0}'; move it to a folder named 'When_<condition>'",
        category: TestingCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor TestFileMustContainSingleTest = new(
        id: "TEST003",
        title: "Test file must contain a single test",
        messageFormat: "Test '{0}' is not the only test in this file; each 'Then_' file contains exactly one [Fact] or [Theory]",
        category: TestingCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor TestNamespaceMustEndWithWhenFolder = new(
        id: "TEST004",
        title: "Test namespace must end with its 'When_' folder",
        messageFormat: "Namespace '{0}' must end with '.{1}'",
        category: TestingCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor TestMethodMustBeNamedAfterClass = new(
        id: "TEST005",
        title: "Test method must be named '<ClassName>_Run'",
        messageFormat: "Test method '{0}' must be named '{1}'",
        category: TestingCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor CoreMustNotDependOnInfrastructure = new(
        id: "ARCH001",
        title: "Core must not depend on infrastructure or web frameworks",
        messageFormat: "Core cannot use '{0}'; define an abstraction in Core and implement it in Infrastructure",
        category: ArchitectureCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor IgnoreQueryFiltersOnlyInInfrastructure = new(
        id: "ARCH002",
        title: "IgnoreQueryFilters is only allowed in Infrastructure",
        messageFormat: "IgnoreQueryFilters() bypasses tenant isolation and is only allowed in Infrastructure",
        category: ArchitectureCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor SecurityMustNotDependOnApplication = new(
        id: "ARCH003",
        title: "Security must not depend on the application",
        messageFormat: "Security cannot use '{0}'; keep it reusable by adding an abstraction in Security and implementing it in Infrastructure",
        category: ArchitectureCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor TenancyMustNotDependOnApplication = new(
        id: "ARCH004",
        title: "Tenancy must not depend on the application",
        messageFormat: "Tenancy cannot use '{0}'; keep it reusable by adding an abstraction in Tenancy and implementing it in the application",
        category: ArchitectureCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ImportExportMustNotDependOnApplication = new(
        id: "ARCH005",
        title: "ImportExport must not depend on the application",
        messageFormat: "ImportExport cannot use '{0}'; keep the engine free of business concepts and implement module profiles in Core",
        category: ArchitectureCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NotificationsMustNotDependOnApplication = new(
        id: "ARCH006",
        title: "Notifications must not depend on the application",
        messageFormat: "Notifications cannot use '{0}'; keep emails and web push free of business concepts and decide recipients and texts in the application",
        category: ArchitectureCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor SubscriptionsMustNotDependOnApplication = new(
        id: "ARCH007",
        title: "Subscriptions must not depend on the application",
        messageFormat: "Subscriptions cannot use '{0}'; keep plans and features free of business concepts and resolve the subscriber in the application",
        category: ArchitectureCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
