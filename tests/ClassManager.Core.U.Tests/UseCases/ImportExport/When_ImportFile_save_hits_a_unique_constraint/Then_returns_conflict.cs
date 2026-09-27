using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_ImportFile_save_hits_a_unique_constraint;

public sealed class Then_returns_conflict
{
    [Fact]
    public async Task Then_returns_conflict_Run()
    {
        var builder = new ImportUseCaseBuilder();
        builder.UnitOfWork
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UniqueConstraintViolationException());
        var command = new ImportFileCommand(InstructorImportModule.ModuleName, ImportUseCaseBuilder.Csv("Profesor\nMarta Ruiz\n"));

        var report = await builder.ImportUseCase.ExecuteAsync(command, CancellationToken.None);

        report.Error!.Code.ShouldBe(ImportUseCaseErrorCodes.ConcurrentChange);
    }
}
