namespace ClassManager.ImportExport.U.Tests.Tabular.Xlsx.When_workbook_declares_a_document_type;

public sealed class Then_unreadable_file_error_is_returned
{
    [Fact]
    public void Then_unreadable_file_error_is_returned_Run()
    {
        var content = XlsxFiles.Zip(new Dictionary<string, string>
        {
            ["xl/workbook.xml"] = """<!DOCTYPE workbook [<!ENTITY bomb "boom">]><workbook><sheets/></workbook>""",
        });

        var result = new XlsxTabularReader().Read(content);

        result.Error!.Code.ShouldBe(ImportErrorCodes.UnreadableFile);
    }
}
