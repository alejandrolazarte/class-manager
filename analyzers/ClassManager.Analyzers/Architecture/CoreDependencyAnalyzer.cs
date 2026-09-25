using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ClassManager.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CoreDependencyAnalyzer : DiagnosticAnalyzer
{
    private const string CoreAssemblyName = "Core";
    private const string CoreAssemblySuffix = ".Core";
    private const string NamespaceSeparator = ".";

    private static readonly ImmutableArray<string> ForbiddenNamespaces = ImmutableArray.Create(
        "Microsoft.EntityFrameworkCore",
        "Microsoft.AspNetCore",
        "ClassManager.Infrastructure",
        "ClassManager.Api");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.CoreMustNotDependOnInfrastructure);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationStartAction(compilationStartContext =>
        {
            if (IsCoreAssembly(compilationStartContext.Compilation.AssemblyName))
            {
                compilationStartContext.RegisterSyntaxNodeAction(AnalyzeUsingDirective, SyntaxKind.UsingDirective);
            }
        });
    }

    private static void AnalyzeUsingDirective(SyntaxNodeAnalysisContext context)
    {
        var usingDirective = (UsingDirectiveSyntax)context.Node;
        var namespaceName = usingDirective.Name?.ToString();
        if (namespaceName is null || !IsForbidden(namespaceName))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.CoreMustNotDependOnInfrastructure,
            usingDirective.GetLocation(),
            namespaceName));
    }

    private static bool IsForbidden(string namespaceName) =>
        ForbiddenNamespaces.Any(forbiddenNamespace =>
            namespaceName == forbiddenNamespace
            || namespaceName.StartsWith(forbiddenNamespace + NamespaceSeparator, StringComparison.Ordinal));

    private static bool IsCoreAssembly(string? assemblyName) =>
        assemblyName == CoreAssemblyName
        || (assemblyName?.EndsWith(CoreAssemblySuffix, StringComparison.Ordinal) ?? false);
}
