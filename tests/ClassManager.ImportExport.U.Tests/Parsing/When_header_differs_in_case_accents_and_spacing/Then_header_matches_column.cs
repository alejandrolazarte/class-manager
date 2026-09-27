namespace ClassManager.ImportExport.U.Tests.Parsing.When_header_differs_in_case_accents_and_spacing;

public sealed class Then_header_matches_column
{
    [Theory]
    [InlineData("  TELEFONO ")]
    [InlineData("teléfono")]
    [InlineData("Te-le_fo.no")]
    [InlineData("phone")]
    public void Then_header_matches_column_Run(string header)
    {
        var mapping = HeaderMatcher.Match([TestFiles.NameHeader, header], TestFiles.Columns);

        mapping.Headers[1].Key.ShouldBe(TestFiles.PhoneKey);
    }
}
