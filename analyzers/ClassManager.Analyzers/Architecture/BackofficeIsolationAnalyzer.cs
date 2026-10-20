using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ClassManager.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class BackofficeIsolationAnalyzer : LibraryIndependenceAnalyzer
{
    private static readonly ImmutableArray<string> CustomerAppAssemblyNames = ImmutableArray.Create("Api", "Core");

    private static readonly ImmutableArray<string> BackofficeNamespaces = ImmutableArray.Create("ClassManager.Backoffice");

    protected override DiagnosticDescriptor Descriptor => DiagnosticDescriptors.AppMustNotDependOnBackoffice;

    protected override ImmutableArray<string> ForbiddenNamespaces => BackofficeNamespaces;

    protected override bool IsLibraryAssembly(string? assemblyName) =>
        assemblyName is not null && CustomerAppAssemblyNames.Contains(assemblyName);
}
