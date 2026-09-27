namespace ClassManager.ImportExport.U.Tests.Parsing.When_required_header_is_missing;

public sealed class Then_missing_column_is_reported
{
    [Fact]
    public void Then_missing_column_is_reported_Run()
    {
        var mapping = HeaderMatcher.Match([TestFiles.NameHeader, TestFiles.BirthDateHeader], TestFiles.Columns);

        mapping.MissingRequiredColumns.Select(column => column.Key).ShouldBe([TestFiles.PhoneKey]);
    }
}
