using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_Export_instructors;

public sealed class Then_file_includes_each_email
{
    [Fact]
    public async Task Then_file_includes_each_email_Run()
    {
        var builder = new ImportUseCaseBuilder(TestData.InstructorFullName);
        builder.ExistingInstructors[0].Update(TestData.InstructorFullName, "laura@example.com");

        var file = await builder.ExportUseCase.ExecuteAsync(new ExportQuery(InstructorImportModule.ModuleName), CancellationToken.None);

        ImportUseCaseBuilder.Lines(file.Value!).ShouldBe(["Profesor;Email", $"{TestData.InstructorFullName};laura@example.com"]);
    }
}
