using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_Export_instructors;

public sealed class Then_file_lists_every_instructor_with_a_dated_name
{
    [Fact]
    public async Task Then_file_lists_every_instructor_with_a_dated_name_Run()
    {
        var builder = new ImportUseCaseBuilder(TestData.InstructorFullName, "Marta Ruiz");
        builder.ExistingInstructors[1].Deactivate();

        var file = await builder.ExportUseCase.ExecuteAsync(new ExportQuery(InstructorImportModule.ModuleName), CancellationToken.None);

        ImportUseCaseBuilder.Lines(file.Value!).ShouldBe(["Profesor", TestData.InstructorFullName, "Marta Ruiz"]);
        file.Value!.FileName.ShouldBe("instructors-2026-09-24.csv");
    }
}
