namespace ClassManager.ImportExport.U.Tests.Tabular.Csv.When_cell_is_quoted;

public sealed class Then_delimiters_quotes_and_line_breaks_are_kept
{
    [Fact]
    public void Then_delimiters_quotes_and_line_breaks_are_kept_Run()
    {
        var data = TestFiles.Read("Alumno;Notas\r\nAna;\"Nada; \"\"nunca\"\"\r\nsola\"\r\n");

        data.Rows[0].Cells[1].ShouldBe("Nada; \"nunca\"\r\nsola");
    }
}
