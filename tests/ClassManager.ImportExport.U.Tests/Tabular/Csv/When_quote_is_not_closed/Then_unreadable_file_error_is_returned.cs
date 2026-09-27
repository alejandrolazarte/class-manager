using ClassManager.ImportExport;

namespace ClassManager.ImportExport.U.Tests.Tabular.Csv.When_quote_is_not_closed;

public sealed class Then_unreadable_file_error_is_returned
{
    [Fact]
    public void Then_unreadable_file_error_is_returned_Run()
    {
        var result = new CsvTabularReader().Read(TestFiles.Utf8("Alumno;Notas\nAna;\"sin cerrar\n"));

        result.Error!.Code.ShouldBe(ImportErrorCodes.UnreadableFile);
    }
}
