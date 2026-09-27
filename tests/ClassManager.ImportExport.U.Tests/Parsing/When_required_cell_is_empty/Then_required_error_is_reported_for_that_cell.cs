using ClassManager.ImportExport;

namespace ClassManager.ImportExport.U.Tests.Parsing.When_required_cell_is_empty;

public sealed class Then_required_error_is_reported_for_that_cell
{
    [Fact]
    public async Task Then_required_error_is_reported_for_that_cell_Run()
    {
        var result = await TestFiles.ParseAsync("Alumno;Teléfono\nAna;  \n");

        result.Import!.Rows[0].Errors.ShouldBe([new ImportCellError(TestFiles.PhoneKey, ImportErrorCodes.Required, "Teléfono is required.")]);
    }
}
