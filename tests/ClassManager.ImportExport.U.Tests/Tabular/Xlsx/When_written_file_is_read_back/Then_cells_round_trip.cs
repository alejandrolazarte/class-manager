namespace ClassManager.ImportExport.U.Tests.Tabular.Xlsx.When_written_file_is_read_back;

public sealed class Then_cells_round_trip
{
    [Fact]
    public async Task Then_cells_round_trip_Run()
    {
        string[] cells = ["María; \"Mari\"", "=peligro", "dos\r\nlíneas", "+34611222333", "  espacios  "];
        var content = await XlsxFiles.WriteAsync(["A", "B", "C", "D", "E"], cells);

        var data = XlsxFiles.Read(content);

        data.Rows[0].Cells.ShouldBe(cells);
    }
}
