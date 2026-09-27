namespace ClassManager.ImportExport.U.Tests.Tabular.Xlsx.When_row_skips_cells;

public sealed class Then_cells_keep_their_column
{
    [Fact]
    public void Then_cells_keep_their_column_Run()
    {
        var content = XlsxFiles.Workbook(
            """
            <row r="1"><c r="A1" t="inlineStr"><is><t>A</t></is></c><c r="B1" t="inlineStr"><is><t>B</t></is></c><c r="C1" t="inlineStr"><is><t>C</t></is></c></row>
            <row r="2"><c r="C2" t="inlineStr"><is><t>tercera</t></is></c></row>
            """);

        XlsxFiles.Read(content).Rows[0].Cells.ShouldBe([string.Empty, string.Empty, "tercera"]);
    }
}
