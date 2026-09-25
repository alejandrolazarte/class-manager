using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ClassManager.Analyzers.TestLayout;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TestLayoutAnalyzer : DiagnosticAnalyzer
{
    private const string WhenFolderPrefix = "When_";
    private const string ThenFilePrefix = "Then_";
    private const string TestMethodSuffix = "_Run";
    private const string NamespaceSeparator = ".";
    private const char QualifiedNameSeparator = '.';
    private const string AttributeSuffix = "Attribute";

    private static readonly ImmutableHashSet<string> TestAttributeNames = ImmutableHashSet.Create("Fact", "Theory");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        DiagnosticDescriptors.TestFileNameMustStartWithThen,
        DiagnosticDescriptors.TestFileMustBeInWhenFolder,
        DiagnosticDescriptors.TestFileMustContainSingleTest,
        DiagnosticDescriptors.TestNamespaceMustEndWithWhenFolder,
        DiagnosticDescriptors.TestMethodMustBeNamedAfterClass);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
    }

    private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
    {
        var filePath = context.Tree.FilePath;
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        var testMethods = context.Tree.GetRoot(context.CancellationToken)
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Where(IsTestMethod)
            .ToList();

        if (testMethods.Count == 0)
        {
            return;
        }

        var firstTestMethod = testMethods[0];
        var firstTestLocation = firstTestMethod.Identifier.GetLocation();

        ReportFileNameViolation(context, filePath, firstTestLocation);
        ReportFolderAndNamespaceViolations(context, filePath, firstTestMethod, firstTestLocation);
        ReportExtraTests(context, testMethods);
        ReportMethodNameViolation(context, firstTestMethod, firstTestLocation);
    }

    private static void ReportFileNameViolation(SyntaxTreeAnalysisContext context, string filePath, Location location)
    {
        if (!Path.GetFileName(filePath).StartsWith(ThenFilePrefix, StringComparison.Ordinal))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.TestFileNameMustStartWithThen,
                location,
                Path.GetFileName(filePath)));
        }
    }

    private static void ReportFolderAndNamespaceViolations(
        SyntaxTreeAnalysisContext context,
        string filePath,
        MethodDeclarationSyntax firstTestMethod,
        Location location)
    {
        var folderName = Path.GetFileName(Path.GetDirectoryName(filePath)) ?? string.Empty;
        if (!folderName.StartsWith(WhenFolderPrefix, StringComparison.Ordinal))
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.TestFileMustBeInWhenFolder, location, folderName));
            return;
        }

        var namespaceName = firstTestMethod.Ancestors()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .FirstOrDefault()?.Name.ToString() ?? string.Empty;

        if (!namespaceName.EndsWith(NamespaceSeparator + folderName, StringComparison.Ordinal))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.TestNamespaceMustEndWithWhenFolder,
                location,
                namespaceName,
                folderName));
        }
    }

    private static void ReportExtraTests(SyntaxTreeAnalysisContext context, IReadOnlyList<MethodDeclarationSyntax> testMethods)
    {
        foreach (var extraTestMethod in testMethods.Skip(1))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.TestFileMustContainSingleTest,
                extraTestMethod.Identifier.GetLocation(),
                extraTestMethod.Identifier.Text));
        }
    }

    private static void ReportMethodNameViolation(
        SyntaxTreeAnalysisContext context,
        MethodDeclarationSyntax firstTestMethod,
        Location location)
    {
        if (firstTestMethod.Parent is not TypeDeclarationSyntax containingType)
        {
            return;
        }

        var expectedMethodName = containingType.Identifier.Text + TestMethodSuffix;
        if (firstTestMethod.Identifier.Text != expectedMethodName)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.TestMethodMustBeNamedAfterClass,
                location,
                firstTestMethod.Identifier.Text,
                expectedMethodName));
        }
    }

    private static bool IsTestMethod(MethodDeclarationSyntax method) =>
        method.AttributeLists
            .SelectMany(attributeList => attributeList.Attributes)
            .Select(attribute => ToSimpleAttributeName(attribute.Name.ToString()))
            .Any(TestAttributeNames.Contains);

    private static string ToSimpleAttributeName(string attributeName)
    {
        var simpleName = attributeName.Substring(attributeName.LastIndexOf(QualifiedNameSeparator) + 1);

        return simpleName.EndsWith(AttributeSuffix, StringComparison.Ordinal)
            ? simpleName.Substring(0, simpleName.Length - AttributeSuffix.Length)
            : simpleName;
    }
}
