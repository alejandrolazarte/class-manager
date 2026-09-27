namespace ClassManager.ImportExport.U.Tests.Parsing.When_header_is_an_alias;

public sealed class Then_header_matches_column
{
    [Fact]
    public void Then_header_matches_column_Run()
    {
        var mapping = HeaderMatcher.Match(["Nombre", "Móvil"], TestFiles.Columns);

        mapping.Headers.Select(header => header.Key).ShouldBe([TestFiles.NameKey, TestFiles.PhoneKey]);
    }
}
