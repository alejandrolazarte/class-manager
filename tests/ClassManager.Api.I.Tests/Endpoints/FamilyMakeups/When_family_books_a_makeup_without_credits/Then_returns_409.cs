using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyMakeups.When_family_books_a_makeup_without_credits;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        using var response = await scenario.Family.PutMakeupAsync(
            scenario.Coaches.CoachStudentId, scenario.Coaches.OtherClassGroup.Id, CoachScenario.ClassDate);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await response.Content.ReadAsStringAsync()).ShouldContain(SessionErrorCodes.MakeupNoCredit);
    }
}
