using ClassManager.Core.UseCases.ImportExport.Students;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_exports_a_child;

public sealed class Then_row_has_the_family_contact_and_formatted_values
{
    [Fact]
    public async Task Then_row_has_the_family_contact_and_formatted_values_Run()
    {
        var builder = new StudentImportBuilder();
        var client = builder.AddExistingClient("María Gómez", "11 7777-8888", "maria@example.com", "Paga en efectivo");
        builder.AddExistingStudent(client, "Lucas Gómez", new DateOnly(2015, 3, 7), "Alergia");

        var rows = await builder.Module.ExportRowsAsync(CancellationToken.None);

        rows.Value!.Single().ShouldBe(new Dictionary<string, string?>
        {
            [StudentImportModule.StudentNameKey] = "Lucas Gómez",
            [StudentImportModule.PhoneKey] = "+54 1177778888",
            [StudentImportModule.EmailKey] = "maria@example.com",
            [StudentImportModule.BirthDateKey] = "07/03/2015",
            [StudentImportModule.StudentNotesKey] = "Alergia",
            [StudentImportModule.ContactNameKey] = "María Gómez",
            [StudentImportModule.ContactNotesKey] = "Paga en efectivo",
        });
    }
}
