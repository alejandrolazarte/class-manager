using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ClassManager.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SecurityIndependenceAnalyzer : LibraryIndependenceAnalyzer
{
    private const string SecurityAssemblyName = "Security";
    private const string SecurityAssemblySuffix = ".Security";

    private static readonly ImmutableArray<string> ApplicationNamespaces = ImmutableArray.Create(
        "ClassManager.Core",
        "ClassManager.Infrastructure",
        "ClassManager.Api");

    protected override DiagnosticDescriptor Descriptor => DiagnosticDescriptors.SecurityMustNotDependOnApplication;

    protected override ImmutableArray<string> ForbiddenNamespaces => ApplicationNamespaces;

    protected override bool IsLibraryAssembly(string? assemblyName) =>
        assemblyName == SecurityAssemblyName
        || (assemblyName?.EndsWith(SecurityAssemblySuffix, StringComparison.Ordinal) ?? false);
}
