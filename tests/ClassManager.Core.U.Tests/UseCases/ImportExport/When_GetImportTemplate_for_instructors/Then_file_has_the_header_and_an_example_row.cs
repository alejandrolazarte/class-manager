using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_GetImportTemplate_for_instructors;

public sealed class Then_file_has_the_header_and_an_example_row
{
    [Fact]
    public async Task Then_file_has_the_header_and_an_example_row_Run()
    {
        var builder = new ImportUseCaseBuilder();

        var file = await builder.TemplateUseCase.ExecuteAsync(new GetImportTemplateQuery(InstructorImportModule.ModuleName), CancellationToken.None);

        ImportUseCaseBuilder.Lines(file.Value!).ShouldBe(["Profesor", "Laura Gómez"]);
        file.Value!.FileName.ShouldBe("instructors-template.csv");
    }
}
