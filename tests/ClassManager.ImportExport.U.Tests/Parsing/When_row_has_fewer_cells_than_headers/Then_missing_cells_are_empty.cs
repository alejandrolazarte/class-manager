namespace ClassManager.ImportExport.U.Tests.Parsing.When_row_has_fewer_cells_than_headers;

public sealed class Then_missing_cells_are_empty
{
    [Fact]
    public async Task Then_missing_cells_are_empty_Run()
    {
        var result = await TestFiles.ParseAsync("Teléfono;Alumno;Nacimiento\n611;Ana\n");

        result.Import!.Rows[0].GetDate(TestFiles.BirthDateKey).ShouldBeNull();
        result.Import.Rows[0].Errors.ShouldBeEmpty();
    }
}
