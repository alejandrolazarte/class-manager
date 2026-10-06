using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace ClassManager.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnnamedIgnoreQueryFiltersAnalyzer : DiagnosticAnalyzer
{
    private const string IgnoreQueryFiltersMethodName = "IgnoreQueryFilters";
    private const string QueryableExtensionsTypeName = "Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions";
    private const int SourceOnlyParameterCount = 1;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.IgnoreQueryFiltersMustNameFilters);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterOperationAction(AnalyzeInvocation, OperationKind.Invocation);
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context)
    {
        var invocation = (IInvocationOperation)context.Operation;
        var targetMethod = invocation.TargetMethod;

        if (targetMethod.Name == IgnoreQueryFiltersMethodName
            && targetMethod.ContainingType.ToDisplayString() == QueryableExtensionsTypeName
            && targetMethod.Parameters.Length == SourceOnlyParameterCount)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.IgnoreQueryFiltersMustNameFilters,
                invocation.Syntax.GetLocation()));
        }
    }
}
