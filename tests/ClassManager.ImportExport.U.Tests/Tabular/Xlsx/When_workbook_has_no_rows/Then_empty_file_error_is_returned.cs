namespace ClassManager.ImportExport.U.Tests.Tabular.Xlsx.When_workbook_has_no_rows;

public sealed class Then_empty_file_error_is_returned
{
    [Fact]
    public void Then_empty_file_error_is_returned_Run()
    {
        var result = new XlsxTabularReader().Read(XlsxFiles.Workbook(string.Empty));

        result.Error!.Code.ShouldBe(ImportErrorCodes.EmptyFile);
    }
}
