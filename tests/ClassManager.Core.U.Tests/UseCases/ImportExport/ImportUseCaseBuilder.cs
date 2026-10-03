using System.Text;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;
using ClassManager.ImportExport;
using ClassManager.ImportExport.Parsing;
using ClassManager.ImportExport.Tabular.Csv;
using ClassManager.Subscriptions.Access;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport;

internal sealed class ImportUseCaseBuilder
{
    public const string UnknownModule = "payments";

    public Mock<IInstructorRepository> Instructors { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public List<Instructor> AddedInstructors { get; } = [];
    public ImportParser Parser { get; } = new(new CsvTabularReader());
    public List<Instructor> ExistingInstructors { get; }

    public ImportUseCaseBuilder(params string[] existingInstructorNames)
    {
        ExistingInstructors = existingInstructorNames.Select(name => Instructor.Create(name).Value!).ToList();
        var existingInstructors = ExistingInstructors;
        Instructors.Setup(repository => repository.ListAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(existingInstructors);
        Instructors.Setup(repository => repository.Add(It.IsAny<Instructor>())).Callback<Instructor>(AddedInstructors.Add);
    }

    public InstructorImportModule InstructorModule => new(Instructors.Object);

    public PreviewImportUseCase PreviewUseCase => new([InstructorModule], Parser, Mock.Of<IFeatureAccess>(), Mock.Of<IFeatureUsage>());

    public ImportFileUseCase ImportUseCase => new([InstructorModule], Parser, UnitOfWork.Object, Mock.Of<IFeatureAccess>(), Mock.Of<IFeatureUsage>());

    public ExportUseCase ExportUseCase => new([InstructorModule], new CsvTabularWriter(), new FakeTimeProvider(TestData.Now));

    public GetImportTemplateUseCase TemplateUseCase => new([InstructorModule], new CsvTabularWriter());

    public static string[] Lines(ExportFile file) =>
        Encoding.UTF8.GetString(file.Content).TrimStart('\uFEFF').Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

    public static MemoryStream Csv(string text) => new(Encoding.UTF8.GetBytes(text));

    public async Task<ImportPlan> PlanInstructorsAsync(string csvText)
    {
        var parsed = await Parser.ParseAsync(Csv(csvText), InstructorModule.Columns, ImportLimits.Default, CancellationToken.None);
        var plan = await InstructorModule.PlanAsync(parsed.Import!.Rows, CancellationToken.None);
        return plan.Value!;
    }
}
