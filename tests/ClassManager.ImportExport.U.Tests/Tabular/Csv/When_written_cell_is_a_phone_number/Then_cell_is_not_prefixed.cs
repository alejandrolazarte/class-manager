namespace ClassManager.ImportExport.U.Tests.Tabular.Csv.When_written_cell_is_a_phone_number;

public sealed class Then_cell_is_not_prefixed
{
    [Theory]
    [InlineData("+34 611 22 23 33")]
    [InlineData("-")]
    public async Task Then_cell_is_not_prefixed_Run(string value)
    {
        var output = await TestFiles.WriteAsync(["Teléfono"], [value]);

        output.ShouldBe("﻿Teléfono\r\n" + value + "\r\n");
    }
}
