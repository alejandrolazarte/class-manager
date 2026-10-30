using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_a_student_fixes_their_birth_date;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_business_has_the_new_birth_date(ApiFixture fixture)
{
    private static readonly DateOnly NewBirthDate = new(1985, 7, 21);

    [Fact]
    public async Task Then_the_business_has_the_new_birth_date_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        using (var response = await scenario.Student.PutMyProfileAsync("Ana María Pérez", NewBirthDate))
        {
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        await using var context = fixture.CreateDbContext(scenario.Instructors.Business.Business.Id);
        var birthDate = await context.Clients
            .Where(client => client.Id == scenario.ClientId)
            .Select(client => client.User.BirthDate)
            .SingleAsync();

        birthDate.ShouldBe(NewBirthDate);
    }
}
