namespace ClassManager.ImportExport.U.Tests.Parsing.When_header_is_unknown;

public sealed class Then_header_is_not_mapped
{
    [Fact]
    public void Then_header_is_not_mapped_Run()
    {
        var mapping = HeaderMatcher.Match([TestFiles.NameHeader, TestFiles.PhoneHeader, "Grupo"], TestFiles.Columns);

        mapping.Headers[2].Key.ShouldBeNull();
    }
}
