namespace ClassManager.ImportExport.U.Tests.Parsing.When_file_has_too_many_rows;

public sealed class Then_too_many_rows_error_is_returned
{
    [Fact]
    public async Task Then_too_many_rows_error_is_returned_Run()
    {
        var result = await TestFiles.ParseAsync("Alumno;Teléfono\nAna;611\nLuis;612\nEva;613\n", new ImportLimits(MaximumFileSizeInBytes: 1000, MaximumRowCount: 2));

        result.Error!.Code.ShouldBe(ImportErrorCodes.TooManyRows);
    }
}
