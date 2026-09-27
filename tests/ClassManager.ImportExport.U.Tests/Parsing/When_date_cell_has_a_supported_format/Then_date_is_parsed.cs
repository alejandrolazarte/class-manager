namespace ClassManager.ImportExport.U.Tests.Parsing.When_date_cell_has_a_supported_format;

public sealed class Then_date_is_parsed
{
    [Theory]
    [InlineData("07/03/2015")]
    [InlineData("7/3/2015")]
    [InlineData("7-3-2015")]
    [InlineData("2015-03-07")]
    public async Task Then_date_is_parsed_Run(string value)
    {
        var result = await TestFiles.ParseAsync($"Alumno;Teléfono;Nacimiento\nAna;611;{value}\n");

        result.Import!.Rows[0].GetDate(TestFiles.BirthDateKey).ShouldBe(new DateOnly(2015, 3, 7));
    }
}
