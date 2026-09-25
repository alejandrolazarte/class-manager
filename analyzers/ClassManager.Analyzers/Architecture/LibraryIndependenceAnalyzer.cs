using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ClassManager.Analyzers.Architecture;

public abstract class LibraryIndependenceAnalyzer : DiagnosticAnalyzer
{
    private const string NamespaceSeparator = ".";

    protected abstract DiagnosticDescriptor Descriptor { get; }

    protected abstract ImmutableArray<string> ForbiddenNamespaces { get; }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationStartAction(compilationStartContext =>
        {
            if (IsLibraryAssembly(compilationStartContext.Compilation.AssemblyName))
            {
                compilationStartContext.RegisterSyntaxNodeAction(AnalyzeUsingDirective, SyntaxKind.UsingDirective);
            }
        });
    }

    protected abstract bool IsLibraryAssembly(string? assemblyName);

    private void AnalyzeUsingDirective(SyntaxNodeAnalysisContext context)
    {
        var usingDirective = (UsingDirectiveSyntax)context.Node;
        var namespaceName = usingDirective.Name?.ToString();
        if (namespaceName is null || !IsForbidden(namespaceName))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptor, usingDirective.GetLocation(), namespaceName));
    }

    private bool IsForbidden(string namespaceName) =>
        ForbiddenNamespaces.Any(forbiddenNamespace =>
            namespaceName == forbiddenNamespace
            || namespaceName.StartsWith(forbiddenNamespace + NamespaceSeparator, StringComparison.Ordinal));
}
