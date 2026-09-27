using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_ImportFile_has_valid_rows;

public sealed class Then_they_are_saved_once
{
    [Fact]
    public async Task Then_they_are_saved_once_Run()
    {
        var builder = new ImportUseCaseBuilder(TestData.InstructorFullName);
        var command = new ImportFileCommand(InstructorImportModule.ModuleName, ImportUseCaseBuilder.Csv("Profesor\nMarta Ruiz\nLaura Gómez\n"));

        var report = await builder.ImportUseCase.ExecuteAsync(command, CancellationToken.None);

        report.Value!.Summary.Valid.ShouldBe(1);
        builder.AddedInstructors.Count.ShouldBe(1);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
