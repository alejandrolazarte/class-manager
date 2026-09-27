using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ClassManager.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ImportExportIndependenceAnalyzer : LibraryIndependenceAnalyzer
{
    private const string ImportExportAssemblyName = "ImportExport";
    private const string ImportExportAssemblyPrefix = "ImportExport.";
    private const string ImportExportAssemblySegment = ".ImportExport";

    private static readonly ImmutableArray<string> ApplicationNamespaces = ImmutableArray.Create(
        "ClassManager.Core",
        "ClassManager.Infrastructure",
        "ClassManager.Api",
        "ClassManager.Security",
        "ClassManager.Tenancy");

    protected override DiagnosticDescriptor Descriptor => DiagnosticDescriptors.ImportExportMustNotDependOnApplication;

    protected override ImmutableArray<string> ForbiddenNamespaces => ApplicationNamespaces;

    protected override bool IsLibraryAssembly(string? assemblyName) =>
        assemblyName is not null
        && (assemblyName == ImportExportAssemblyName
            || assemblyName.StartsWith(ImportExportAssemblyPrefix, StringComparison.Ordinal)
            || assemblyName.EndsWith(ImportExportAssemblySegment, StringComparison.Ordinal)
            || assemblyName.Contains(ImportExportAssemblySegment + "."));
}
