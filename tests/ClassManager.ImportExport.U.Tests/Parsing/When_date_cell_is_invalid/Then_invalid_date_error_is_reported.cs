namespace ClassManager.ImportExport.U.Tests.Parsing.When_date_cell_is_invalid;

public sealed class Then_invalid_date_error_is_reported
{
    [Theory]
    [InlineData("31/02/2015")]
    [InlineData("03/07/15")]
    [InlineData("ayer")]
    public async Task Then_invalid_date_error_is_reported_Run(string value)
    {
        var result = await TestFiles.ParseAsync($"Alumno;Teléfono;Nacimiento\nAna;611;{value}\n");

        result.Import!.Rows[0].Errors.Single().Code.ShouldBe(ImportErrorCodes.InvalidDate);
    }
}
