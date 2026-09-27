using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_PreviewImport_runs;

public sealed class Then_report_counts_rows_and_nothing_is_saved
{
    [Fact]
    public async Task Then_report_counts_rows_and_nothing_is_saved_Run()
    {
        var builder = new ImportUseCaseBuilder(TestData.InstructorFullName);
        var command = new PreviewImportCommand(InstructorImportModule.ModuleName, ImportUseCaseBuilder.Csv("Profesor;Notas\nMarta Ruiz;x\nLaura Gómez;y\n;z\n"));

        var report = await builder.PreviewUseCase.ExecuteAsync(command, CancellationToken.None);

        report.Value!.Summary.ShouldBe(new ImportSummary(Total: 3, Valid: 1, Errors: 1, Skipped: 1));
        builder.Instructors.Verify(repository => repository.Add(It.IsAny<Instructor>()), Times.Never);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
