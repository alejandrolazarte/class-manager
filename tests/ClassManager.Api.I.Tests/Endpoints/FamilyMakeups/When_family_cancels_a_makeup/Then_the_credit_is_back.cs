namespace ClassManager.Api.I.Tests.Endpoints.FamilyMakeups.When_family_cancels_a_makeup;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_credit_is_back(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_credit_is_back_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        await scenario.NoticeAndBookOtherClassAsync();

        using var response = await scenario.Family.DeleteMakeupAsync(
            scenario.Coaches.CoachStudentId, scenario.Coaches.OtherClassGroup.Id, CoachScenario.ClassDate);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await scenario.Family.GetMakeupsAsync(scenario.Coaches.CoachStudentId))!.Credits.ShouldHaveSingleItem();
    }
}
