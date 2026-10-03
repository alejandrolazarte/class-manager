using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ClassManager.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RecordsIndependenceAnalyzer : LibraryIndependenceAnalyzer
{
    private const string RecordsAssemblyName = "Records";
    private const string RecordsAssemblyPrefix = "Records.";
    private const string RecordsAssemblySegment = ".Records";

    private static readonly ImmutableArray<string> ApplicationNamespaces = ImmutableArray.Create(
        "ClassManager.Core",
        "ClassManager.Infrastructure",
        "ClassManager.Api",
        "ClassManager.Security",
        "ClassManager.Tenancy",
        "ClassManager.ImportExport",
        "ClassManager.Notifications",
        "ClassManager.Storage",
        "ClassManager.Subscriptions");

    protected override DiagnosticDescriptor Descriptor => DiagnosticDescriptors.RecordsMustNotDependOnApplication;

    protected override ImmutableArray<string> ForbiddenNamespaces => ApplicationNamespaces;

    protected override bool IsLibraryAssembly(string? assemblyName) =>
        assemblyName is not null
        && (assemblyName == RecordsAssemblyName
            || assemblyName.StartsWith(RecordsAssemblyPrefix, StringComparison.Ordinal)
            || assemblyName.EndsWith(RecordsAssemblySegment, StringComparison.Ordinal)
            || assemblyName.Contains(RecordsAssemblySegment + "."));
}
