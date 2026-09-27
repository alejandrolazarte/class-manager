namespace ClassManager.ImportExport.U.Tests.Tabular.Xlsx.When_workbook_has_blank_rows;

public sealed class Then_they_are_skipped_and_line_numbers_are_kept
{
    [Fact]
    public void Then_they_are_skipped_and_line_numbers_are_kept_Run()
    {
        var content = XlsxFiles.Workbook(
            """
            <row r="2"><c r="A2" t="inlineStr"><is><t>Alumno</t></is></c></row>
            <row r="3"><c r="A3" t="inlineStr"><is><t> </t></is></c></row>
            <row r="5"><c r="A5" t="inlineStr"><is><t>Ana</t></is></c></row>
            """);

        var data = XlsxFiles.Read(content);

        data.Headers.ShouldBe(["Alumno"]);
        data.Rows.Select(row => row.LineNumber).ShouldBe([5]);
    }
}
