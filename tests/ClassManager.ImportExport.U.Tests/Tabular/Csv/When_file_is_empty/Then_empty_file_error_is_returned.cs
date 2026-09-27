using ClassManager.ImportExport;

namespace ClassManager.ImportExport.U.Tests.Tabular.Csv.When_file_is_empty;

public sealed class Then_empty_file_error_is_returned
{
    [Theory]
    [InlineData("")]
    [InlineData("\n ; \n")]
    public void Then_empty_file_error_is_returned_Run(string text)
    {
        var result = new CsvTabularReader().Read(TestFiles.Utf8(text));

        result.Error!.Code.ShouldBe(ImportErrorCodes.EmptyFile);
    }
}
