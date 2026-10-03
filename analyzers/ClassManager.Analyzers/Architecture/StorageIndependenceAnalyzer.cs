using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ClassManager.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class StorageIndependenceAnalyzer : LibraryIndependenceAnalyzer
{
    private const string StorageAssemblyName = "Storage";
    private const string StorageAssemblyPrefix = "Storage.";
    private const string StorageAssemblySegment = ".Storage";

    private static readonly ImmutableArray<string> ApplicationNamespaces = ImmutableArray.Create(
        "ClassManager.Core",
        "ClassManager.Infrastructure",
        "ClassManager.Api",
        "ClassManager.Security",
        "ClassManager.Tenancy",
        "ClassManager.ImportExport",
        "ClassManager.Notifications",
        "ClassManager.Subscriptions");

    protected override DiagnosticDescriptor Descriptor => DiagnosticDescriptors.StorageMustNotDependOnApplication;

    protected override ImmutableArray<string> ForbiddenNamespaces => ApplicationNamespaces;

    protected override bool IsLibraryAssembly(string? assemblyName) =>
        assemblyName is not null
        && (assemblyName == StorageAssemblyName
            || assemblyName.StartsWith(StorageAssemblyPrefix, StringComparison.Ordinal)
            || assemblyName.EndsWith(StorageAssemblySegment, StringComparison.Ordinal)
            || assemblyName.Contains(StorageAssemblySegment + "."));
}
