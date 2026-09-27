namespace ClassManager.ImportExport.U.Tests.Tabular.Xlsx.When_rows_are_written;

public sealed class Then_workbook_has_the_headers_and_rows
{
    [Fact]
    public async Task Then_workbook_has_the_headers_and_rows_Run()
    {
        var content = await XlsxFiles.WriteAsync(["Alumno", "Teléfono"], ["Lucas Gómez", "611222333"], ["Ñandú", null]);

        var data = XlsxFiles.Read(content);

        data.Headers.ShouldBe(["Alumno", "Teléfono"]);
        data.Rows[0].Cells.ShouldBe(["Lucas Gómez", "611222333"]);
        data.Rows[1].Cells.ShouldBe(["Ñandú"]);
    }
}
