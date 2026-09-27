namespace ClassManager.ImportExport.U.Tests.Tabular.Csv.When_file_starts_with_utf8_bom;

public sealed class Then_first_header_has_no_bom
{
    [Fact]
    public void Then_first_header_has_no_bom_Run()
    {
        var result = new CsvTabularReader().Read(TestFiles.Utf8WithBom("Alumno;Teléfono\nAna;611\n"));

        result.Data!.Headers.ShouldBe(["Alumno", "Teléfono"]);
    }
}
