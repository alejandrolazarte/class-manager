namespace ClassManager.ImportExport.U.Tests.Tabular.Csv.When_file_has_blank_lines;

public sealed class Then_blank_lines_are_skipped_and_line_numbers_are_kept
{
    [Fact]
    public void Then_blank_lines_are_skipped_and_line_numbers_are_kept_Run()
    {
        var data = TestFiles.Read("Alumno;Notas\n\nAna;\"dos\nlíneas\"\n;\nLuis;x\n");

        data.Rows.Select(row => row.LineNumber).ShouldBe([3, 6]);
    }
}
