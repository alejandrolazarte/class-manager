namespace ClassManager.ImportExport.U.Tests.Tabular.Csv.When_written_cell_has_delimiter_quote_or_line_break;

public sealed class Then_cell_is_quoted
{
    [Fact]
    public async Task Then_cell_is_quoted_Run()
    {
        var output = await TestFiles.WriteAsync(["Notas"], ["a;b"], ["di \"hola\""], ["dos\nlíneas"]);

        output.ShouldBe("﻿Notas\r\n\"a;b\"\r\n\"di \"\"hola\"\"\"\r\n\"dos\nlíneas\"\r\n");
    }
}
