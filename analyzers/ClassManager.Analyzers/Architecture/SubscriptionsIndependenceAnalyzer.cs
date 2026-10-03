using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ClassManager.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SubscriptionsIndependenceAnalyzer : LibraryIndependenceAnalyzer
{
    private const string SubscriptionsAssemblyName = "Subscriptions";
    private const string SubscriptionsAssemblyPrefix = "Subscriptions.";
    private const string SubscriptionsAssemblySegment = ".Subscriptions";

    private static readonly ImmutableArray<string> ApplicationNamespaces = ImmutableArray.Create(
        "ClassManager.Core",
        "ClassManager.Infrastructure",
        "ClassManager.Api",
        "ClassManager.Security",
        "ClassManager.Tenancy",
        "ClassManager.ImportExport",
        "ClassManager.Notifications");

    protected override DiagnosticDescriptor Descriptor => DiagnosticDescriptors.SubscriptionsMustNotDependOnApplication;

    protected override ImmutableArray<string> ForbiddenNamespaces => ApplicationNamespaces;

    protected override bool IsLibraryAssembly(string? assemblyName) =>
        assemblyName is not null
        && (assemblyName == SubscriptionsAssemblyName
            || assemblyName.StartsWith(SubscriptionsAssemblyPrefix, StringComparison.Ordinal)
            || assemblyName.EndsWith(SubscriptionsAssemblySegment, StringComparison.Ordinal)
            || assemblyName.Contains(SubscriptionsAssemblySegment + "."));
}
