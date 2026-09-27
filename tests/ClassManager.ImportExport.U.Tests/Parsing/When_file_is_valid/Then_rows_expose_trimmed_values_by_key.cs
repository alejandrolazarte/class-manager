namespace ClassManager.ImportExport.U.Tests.Parsing.When_file_is_valid;

public sealed class Then_rows_expose_trimmed_values_by_key
{
    [Fact]
    public async Task Then_rows_expose_trimmed_values_by_key_Run()
    {
        var result = await TestFiles.ParseAsync("Grupo;Teléfono;Alumno\nLunes; 611 222 333 ;  Ana Pérez \n");

        var row = result.Import!.Rows[0];
        new[] { row.GetText(TestFiles.NameKey), row.GetText(TestFiles.PhoneKey), row.GetText(TestFiles.BirthDateKey) }
            .ShouldBe(["Ana Pérez", "611 222 333", null]);
        row.LineNumber.ShouldBe(2);
        row.Errors.ShouldBeEmpty();
    }
}
