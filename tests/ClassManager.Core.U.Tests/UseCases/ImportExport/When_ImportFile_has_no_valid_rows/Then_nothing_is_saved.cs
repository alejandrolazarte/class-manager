using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_ImportFile_has_no_valid_rows;

public sealed class Then_nothing_is_saved
{
    [Fact]
    public async Task Then_nothing_is_saved_Run()
    {
        var builder = new ImportUseCaseBuilder(TestData.InstructorFullName);
        var command = new ImportFileCommand(InstructorImportModule.ModuleName, ImportUseCaseBuilder.Csv("Profesor\nLaura Gómez\n"));

        var report = await builder.ImportUseCase.ExecuteAsync(command, CancellationToken.None);

        report.Value!.Summary.Skipped.ShouldBe(1);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
