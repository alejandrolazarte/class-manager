using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ClassManager.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TenancyIndependenceAnalyzer : LibraryIndependenceAnalyzer
{
    private const string TenancyAssemblyName = "Tenancy";
    private const string TenancyAssemblyPrefix = "Tenancy.";
    private const string TenancyAssemblySegment = ".Tenancy";

    private static readonly ImmutableArray<string> ApplicationNamespaces = ImmutableArray.Create(
        "ClassManager.Core",
        "ClassManager.Infrastructure",
        "ClassManager.Api",
        "ClassManager.Security");

    protected override DiagnosticDescriptor Descriptor => DiagnosticDescriptors.TenancyMustNotDependOnApplication;

    protected override ImmutableArray<string> ForbiddenNamespaces => ApplicationNamespaces;

    protected override bool IsLibraryAssembly(string? assemblyName) =>
        assemblyName is not null
        && (assemblyName == TenancyAssemblyName
            || assemblyName.StartsWith(TenancyAssemblyPrefix, StringComparison.Ordinal)
            || assemblyName.EndsWith(TenancyAssemblySegment, StringComparison.Ordinal)
            || assemblyName.Contains(TenancyAssemblySegment + "."));
}
