using System.Globalization;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.UseCases.ImportExport.Students;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_plans_a_minor_alone;

public sealed class Then_row_is_an_error_on_the_birth_date
{
    [Fact]
    public async Task Then_row_is_an_error_on_the_birth_date_Run()
    {
        var builder = new StudentImportBuilder();
        var minorBirthDate = TestData.Today.AddYears(-16).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

        var plan = await builder.PlanAndAddAsync($"Valentina Ríos;11 5555-6666;;{minorBirthDate};\n");

        var error = plan.Rows.Single().Errors.Single();
        error.Key.ShouldBe(StudentImportModule.BirthDateKey);
        error.Code.ShouldBe(ClientErrorCodes.ContactMustBeAdult);
        builder.AddedClients.ShouldBeEmpty();
    }
}
