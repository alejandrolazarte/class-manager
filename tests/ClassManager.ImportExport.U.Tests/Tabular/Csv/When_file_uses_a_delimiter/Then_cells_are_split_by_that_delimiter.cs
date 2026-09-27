namespace ClassManager.ImportExport.U.Tests.Tabular.Csv.When_file_uses_a_delimiter;

public sealed class Then_cells_are_split_by_that_delimiter
{
    [Theory]
    [InlineData("Alumno;Teléfono\nAna;611\n")]
    [InlineData("Alumno,Teléfono\nAna,611\n")]
    [InlineData("Alumno\tTeléfono\nAna\t611\n")]
    public void Then_cells_are_split_by_that_delimiter_Run(string text)
    {
        var data = TestFiles.Read(text);

        data.Rows[0].Cells.ShouldBe(["Ana", "611"]);
    }
}
