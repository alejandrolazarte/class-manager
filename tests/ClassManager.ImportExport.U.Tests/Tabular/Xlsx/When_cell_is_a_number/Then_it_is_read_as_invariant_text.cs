namespace ClassManager.ImportExport.U.Tests.Tabular.Xlsx.When_cell_is_a_number;

public sealed class Then_it_is_read_as_invariant_text
{
    [Fact]
    public void Then_it_is_read_as_invariant_text_Run()
    {
        var content = XlsxFiles.Workbook(
            """
            <row r="1"><c r="A1" t="inlineStr"><is><t>Teléfono</t></is></c><c r="B1" t="inlineStr"><is><t>Cuota</t></is></c><c r="C1" t="inlineStr"><is><t>Activo</t></is></c></row>
            <row r="2"><c r="A2"><v>6.11222333E8</v></c><c r="B2"><v>12.5</v></c><c r="C2" t="b"><v>1</v></c></row>
            """);

        XlsxFiles.Read(content).Rows[0].Cells.ShouldBe(["611222333", "12.5", "TRUE"]);
    }
}
