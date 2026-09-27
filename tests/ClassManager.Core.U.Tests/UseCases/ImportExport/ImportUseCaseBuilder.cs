using System.Text;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;
using ClassManager.ImportExport;
using ClassManager.ImportExport.Parsing;
using ClassManager.ImportExport.Tabular.Csv;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport;

internal sealed class ImportUseCaseBuilder
{
    public const string UnknownModule = "payments";

    public Mock<IInstructorRepository> Instructors { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public List<Instructor> AddedInstructors { get; } = [];
    public ImportParser Parser { get; } = new(new CsvTabularReader());

    public ImportUseCaseBuilder(params string[] existingInstructorNames)
    {
        var existingInstructors = existingInstructorNames.Select(name => Instructor.Create(name).Value!).ToList();
        Instructors.Setup(repository => repository.ListAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(existingInstructors);
        Instructors.Setup(repository => repository.Add(It.IsAny<Instructor>())).Callback<Instructor>(AddedInstructors.Add);
    }

    public InstructorImportModule InstructorModule => new(Instructors.Object);

    public PreviewImportUseCase PreviewUseCase => new([InstructorModule], Parser);

    public ImportFileUseCase ImportUseCase => new([InstructorModule], Parser, UnitOfWork.Object);

    public static MemoryStream Csv(string text) => new(Encoding.UTF8.GetBytes(text));

    public async Task<ImportPlan> PlanInstructorsAsync(string csvText)
    {
        var parsed = await Parser.ParseAsync(Csv(csvText), InstructorModule.Columns, ImportLimits.Default, CancellationToken.None);
        var plan = await InstructorModule.PlanAsync(parsed.Import!.Rows, CancellationToken.None);
        return plan.Value!;
    }
}
