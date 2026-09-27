namespace ClassManager.ImportExport.U.Tests.Tabular.Xlsx.When_a_part_is_larger_than_the_limit;

public sealed class Then_unreadable_file_error_is_returned
{
    [Fact]
    public void Then_unreadable_file_error_is_returned_Run()
    {
        var sheetData = string.Concat(Enumerable.Repeat("""<row><c t="inlineStr"><is><t>Ana</t></is></c></row>""", 200));
        var content = XlsxFiles.Workbook(sheetData);

        var result = new XlsxTabularReader(maximumPartSizeInBytes: 4096).Read(content);

        result.Error!.Code.ShouldBe(ImportErrorCodes.UnreadableFile);
    }
}
