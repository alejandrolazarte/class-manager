namespace ClassManager.ImportExport.U.Tests.Tabular.Xlsx.When_written_cell_has_characters_invalid_in_xml;

public sealed class Then_they_are_removed
{
    [Fact]
    public async Task Then_they_are_removed_Run()
    {
        var content = await XlsxFiles.WriteAsync(["Alumno"], ["An\u0001a\uD800"]);

        XlsxFiles.Read(content).Rows[0].Cells.ShouldBe(["Ana"]);
    }
}
