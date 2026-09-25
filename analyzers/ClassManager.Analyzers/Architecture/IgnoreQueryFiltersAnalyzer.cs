using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace ClassManager.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class IgnoreQueryFiltersAnalyzer : DiagnosticAnalyzer
{
    private const string IgnoreQueryFiltersMethodName = "IgnoreQueryFilters";
    private const string QueryableExtensionsTypeName = "Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions";

    private static readonly ImmutableArray<string> AllowedAssemblySuffixes = ImmutableArray.Create("Infrastructure", "Tests");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.IgnoreQueryFiltersOnlyInInfrastructure);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationStartAction(compilationStartContext =>
        {
            if (!IsAllowedAssembly(compilationStartContext.Compilation.AssemblyName))
            {
                compilationStartContext.RegisterOperationAction(AnalyzeInvocation, OperationKind.Invocation);
            }
        });
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context)
    {
        var invocation = (IInvocationOperation)context.Operation;
        var targetMethod = invocation.TargetMethod;

        if (targetMethod.Name == IgnoreQueryFiltersMethodName
            && targetMethod.ContainingType.ToDisplayString() == QueryableExtensionsTypeName)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.IgnoreQueryFiltersOnlyInInfrastructure,
                invocation.Syntax.GetLocation()));
        }
    }

    private static bool IsAllowedAssembly(string? assemblyName) =>
        assemblyName is not null
        && AllowedAssemblySuffixes.Any(allowedSuffix => assemblyName.EndsWith(allowedSuffix, StringComparison.Ordinal));
}
