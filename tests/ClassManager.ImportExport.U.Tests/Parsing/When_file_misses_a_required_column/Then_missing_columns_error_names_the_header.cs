using ClassManager.ImportExport;

namespace ClassManager.ImportExport.U.Tests.Parsing.When_file_misses_a_required_column;

public sealed class Then_missing_columns_error_names_the_header
{
    [Fact]
    public async Task Then_missing_columns_error_names_the_header_Run()
    {
        var result = await TestFiles.ParseAsync("Alumno;Notas\nAna;x\n");

        result.Error!.Code.ShouldBe(ImportErrorCodes.MissingColumns);
        result.Error.Message.ShouldContain(TestFiles.PhoneHeader);
    }
}
