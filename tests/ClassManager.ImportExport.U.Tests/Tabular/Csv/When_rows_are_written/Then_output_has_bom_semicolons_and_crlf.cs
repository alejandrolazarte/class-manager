namespace ClassManager.ImportExport.U.Tests.Tabular.Csv.When_rows_are_written;

public sealed class Then_output_has_bom_semicolons_and_crlf
{
    [Fact]
    public async Task Then_output_has_bom_semicolons_and_crlf_Run()
    {
        var output = await TestFiles.WriteAsync(["Alumno", "Teléfono"], ["Ana", "+34611222333"], ["Luis", null]);

        output.ShouldBe("﻿Alumno;Teléfono\r\nAna;+34611222333\r\nLuis;\r\n");
    }
}
