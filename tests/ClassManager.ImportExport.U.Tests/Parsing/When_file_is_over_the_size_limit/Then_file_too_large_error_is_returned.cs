using ClassManager.ImportExport;

namespace ClassManager.ImportExport.U.Tests.Parsing.When_file_is_over_the_size_limit;

public sealed class Then_file_too_large_error_is_returned
{
    [Fact]
    public async Task Then_file_too_large_error_is_returned_Run()
    {
        var result = await TestFiles.ParseAsync("Alumno;Teléfono\nAna;611\n", new ImportLimits(MaximumFileSizeInBytes: 10, MaximumRowCount: 10));

        result.Error!.Code.ShouldBe(ImportErrorCodes.FileTooLarge);
    }
}
