namespace ClassManager.ImportExport.U.Tests.Tabular.Csv.When_written_file_is_read_back;

public sealed class Then_cells_round_trip
{
    [Fact]
    public async Task Then_cells_round_trip_Run()
    {
        string?[] cells = ["María; \"Mari\"", "=peligro", "dos\r\nlíneas", "+34611222333"];
        var output = await TestFiles.WriteAsync(["A", "B", "C", "D"], cells);

        var parsed = await new ImportParser(new CsvTabularReader()).ParseAsync(
            new MemoryStream(TestFiles.Utf8(output)),
            [new("a", "A", ColumnType.Text), new("b", "B", ColumnType.Text), new("c", "C", ColumnType.Text), new("d", "D", ColumnType.Phone)],
            ImportLimits.Default,
            CancellationToken.None);

        var row = parsed.Import!.Rows[0];
        new[] { row.GetText("a"), row.GetText("b"), row.GetText("c"), row.GetText("d") }.ShouldBe(cells);
    }
}
