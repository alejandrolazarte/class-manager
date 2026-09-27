namespace ClassManager.ImportExport.U.Tests.Tabular.Xlsx.When_cell_is_a_date_serial;

public sealed class Then_it_is_read_as_an_iso_date
{
    [Fact]
    public void Then_it_is_read_as_an_iso_date_Run()
    {
        var content = XlsxFiles.Workbook(
            """
            <row r="1"><c r="A1" t="inlineStr"><is><t>Built-in</t></is></c><c r="B1" t="inlineStr"><is><t>Custom</t></is></c><c r="C1" t="inlineStr"><is><t>Number</t></is></c></row>
            <row r="2"><c r="A2" s="1"><v>42369</v></c><c r="B2" s="2"><v>42369</v></c><c r="C2" s="3"><v>42369</v></c></row>
            """,
            stylesXml: """
            <numFmts count="2"><numFmt numFmtId="164" formatCode="dd/mm/yyyy;@"/><numFmt numFmtId="165" formatCode="&quot;day&quot; 0"/></numFmts>
            <cellXfs count="4"><xf numFmtId="0"/><xf numFmtId="14"/><xf numFmtId="164"/><xf numFmtId="165"/></cellXfs>
            """);

        XlsxFiles.Read(content).Rows[0].Cells.ShouldBe(["2015-12-31", "2015-12-31", "42369"]);
    }
}
