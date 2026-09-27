namespace ClassManager.ImportExport.U.Tests.Tabular.Xlsx.When_file_is_not_a_workbook;

public sealed class Then_unreadable_file_error_is_returned
{
    public static TheoryData<byte[]> Files =>
    [
        [0x50, 0x4B, 0x03, 0x04, 0x00, 0x01],
        XlsxFiles.Zip(new Dictionary<string, string> { ["content.xml"] = "<document/>" }),
        XlsxFiles.Zip(new Dictionary<string, string> { ["xl/workbook.xml"] = "<workbook><sheets>" }),
    ];

    [Theory]
    [MemberData(nameof(Files))]
    public void Then_unreadable_file_error_is_returned_Run(byte[] content)
    {
        var result = new XlsxTabularReader().Read(content);

        result.Error!.Code.ShouldBe(ImportErrorCodes.UnreadableFile);
    }
}
