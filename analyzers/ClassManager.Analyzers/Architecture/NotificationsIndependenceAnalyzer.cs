using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ClassManager.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NotificationsIndependenceAnalyzer : LibraryIndependenceAnalyzer
{
    private const string NotificationsAssemblyName = "Notifications";
    private const string NotificationsAssemblyPrefix = "Notifications.";
    private const string NotificationsAssemblySegment = ".Notifications";

    private static readonly ImmutableArray<string> ApplicationNamespaces = ImmutableArray.Create(
        "ClassManager.Core",
        "ClassManager.Infrastructure",
        "ClassManager.Api",
        "ClassManager.Security",
        "ClassManager.Tenancy",
        "ClassManager.ImportExport");

    protected override DiagnosticDescriptor Descriptor => DiagnosticDescriptors.NotificationsMustNotDependOnApplication;

    protected override ImmutableArray<string> ForbiddenNamespaces => ApplicationNamespaces;

    protected override bool IsLibraryAssembly(string? assemblyName) =>
        assemblyName is not null
        && (assemblyName == NotificationsAssemblyName
            || assemblyName.StartsWith(NotificationsAssemblyPrefix, StringComparison.Ordinal)
            || assemblyName.EndsWith(NotificationsAssemblySegment, StringComparison.Ordinal)
            || assemblyName.Contains(NotificationsAssemblySegment + "."));
}
