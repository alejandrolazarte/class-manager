using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_the_business_has_another_birth_date;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_profile_shows_it(ApiFixture fixture)
{
    private static readonly DateOnly NewBirthDate = new(1985, 7, 21);

    [Fact]
    public async Task Then_the_profile_shows_it_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        await using (var context = fixture.CreateDbContext(scenario.Instructors.Business.Business.Id))
        {
            var user = await context.Clients.Where(client => client.Id == scenario.ClientId).Select(client => client.User).SingleAsync();
            user.ChangeBirthDate(NewBirthDate, DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime)).IsSuccess.ShouldBeTrue();
            await context.SaveChangesAsync();
        }

        var account = await scenario.Student.GetMyAccountAsync();

        account!.BirthDate.ShouldBe(NewBirthDate);
    }
}
