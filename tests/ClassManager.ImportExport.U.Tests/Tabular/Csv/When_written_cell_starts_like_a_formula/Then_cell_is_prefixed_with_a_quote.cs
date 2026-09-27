namespace ClassManager.ImportExport.U.Tests.Tabular.Csv.When_written_cell_starts_like_a_formula;

public sealed class Then_cell_is_prefixed_with_a_quote
{
    private const string FormulaEscape = "'";
    private const string LineEnding = "\r\n";
    private const char CellQuote = '"';

    [Theory]
    [InlineData("=1+1")]
    [InlineData("@SUM(A1)")]
    [InlineData("+HYPERLINK(\"x\")")]
    [InlineData("-2+3")]
    public async Task Then_cell_is_prefixed_with_a_quote_Run(string value)
    {
        var output = await TestFiles.WriteAsync(["Notas"], [value]);

        output.Split(LineEnding)[1].TrimStart(CellQuote).ShouldStartWith(FormulaEscape + value[0]);
    }
}
