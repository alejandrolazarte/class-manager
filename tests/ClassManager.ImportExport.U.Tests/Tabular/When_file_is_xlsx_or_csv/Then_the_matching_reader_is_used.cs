namespace ClassManager.ImportExport.U.Tests.Tabular.When_file_is_xlsx_or_csv;

public sealed class Then_the_matching_reader_is_used
{
    [Fact]
    public async Task Then_the_matching_reader_is_used_Run()
    {
        var reader = new XlsxOrCsvTabularReader(new XlsxTabularReader(), new CsvTabularReader());
        var workbook = await XlsxFiles.WriteAsync(["Alumno", "Teléfono"], ["Ana", "611"]);
        var csv = TestFiles.Utf8("Alumno;Teléfono\nAna;611\n");

        reader.Read(workbook).Data!.Rows[0].Cells.ShouldBe(["Ana", "611"]);
        reader.Read(csv).Data!.Rows[0].Cells.ShouldBe(["Ana", "611"]);
    }
}
