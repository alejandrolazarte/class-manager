using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace ClassManager.Analyzers.Tests;

internal static class AnalyzerTestRunner
{
    public const string TestAssemblyName = "Api.Tests";
    public const string CoreAssemblyName = "Core";
    public const string InfrastructureAssemblyName = "Infrastructure";
    public const string ApiAssemblyName = "Api";
    public const string SecurityAssemblyName = "Security";
    public const string TenancyAssemblyName = "Tenancy";
    public const string TenancyIntegrationAssemblyName = "Tenancy.AspNetCore";
    public const string ImportExportAssemblyName = "ImportExport";

    private const string XunitStubFilePath = "/stubs/Xunit.cs";
    private const string XunitStubSource = """
        namespace Xunit
        {
            public sealed class FactAttribute : System.Attribute { }
            public sealed class TheoryAttribute : System.Attribute { }
        }
        """;

    private const string EntityFrameworkStubFilePath = "/stubs/EntityFrameworkCore.cs";
    private const string EntityFrameworkStubSource = """
        namespace Microsoft.EntityFrameworkCore
        {
            public static class EntityFrameworkQueryableExtensions
            {
                public static System.Linq.IQueryable<T> IgnoreQueryFilters<T>(this System.Linq.IQueryable<T> source) where T : class => source;
            }
        }
        """;

    public static Task RunAsync<TAnalyzer>(string assemblyName, string filePath, string source)
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        var analyzerTest = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
        };

        analyzerTest.TestState.Sources.Add((filePath, source));
        analyzerTest.TestState.Sources.Add((XunitStubFilePath, XunitStubSource));
        analyzerTest.TestState.Sources.Add((EntityFrameworkStubFilePath, EntityFrameworkStubSource));
        analyzerTest.SolutionTransforms.Add((solution, projectId) => solution.WithProjectAssemblyName(projectId, assemblyName));

        return analyzerTest.RunAsync();
    }
}
