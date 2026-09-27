namespace ClassManager.ImportExport.U.Tests.Parsing.When_two_headers_match_the_same_column;

public sealed class Then_first_header_wins
{
    [Fact]
    public void Then_first_header_wins_Run()
    {
        var mapping = HeaderMatcher.Match([TestFiles.NameHeader, TestFiles.PhoneHeader, "WhatsApp"], TestFiles.Columns);

        mapping.Headers.Select(header => header.Key).ShouldBe([TestFiles.NameKey, TestFiles.PhoneKey, null]);
    }
}
