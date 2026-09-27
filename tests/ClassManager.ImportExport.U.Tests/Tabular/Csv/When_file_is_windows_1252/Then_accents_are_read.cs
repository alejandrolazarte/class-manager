namespace ClassManager.ImportExport.U.Tests.Tabular.Csv.When_file_is_windows_1252;

public sealed class Then_accents_are_read
{
    [Fact]
    public void Then_accents_are_read_Run()
    {
        var result = new CsvTabularReader().Read(TestFiles.Windows1252("Alumno;Teléfono\nMaría Núñez;611\n"));

        result.Data!.Rows[0].Cells[0].ShouldBe("María Núñez");
    }
}
