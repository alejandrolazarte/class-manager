using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyPackClasses.When_family_books_more_classes_than_the_pack_has;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var scenario = await fixture.SeedPackFamilyScenarioAsync(classCount: 1);
        await scenario.BookPackClassAsync();

        using var response = await scenario.Family.PutPackClassAsync(
            scenario.StudentId, scenario.PackClassGroupId, CoachScenario.ClassDate.AddDays(7));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await response.Content.ReadAsStringAsync()).ShouldContain(SessionErrorCodes.PackNoClasses);
    }
}
