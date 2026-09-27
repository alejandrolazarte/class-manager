namespace ClassManager.ImportExport.U.Tests.Tabular.Xlsx.When_cells_use_shared_strings;

public sealed class Then_the_shared_text_is_read
{
    [Fact]
    public void Then_the_shared_text_is_read_Run()
    {
        var content = XlsxFiles.Workbook(
            """<row r="1"><c r="A1" t="s"><v>0</v></c><c r="B1" t="s"><v>1</v></c></row>""",
            """<si><t>Alumno</t></si><si><r><t>Tel</t></r><r><rPr><b/></rPr><t>éfono</t></r><rPh sb="0" eb="1"><t>テ</t></rPh></si>""");

        XlsxFiles.Read(content).Headers.ShouldBe(["Alumno", "Teléfono"]);
    }
}
