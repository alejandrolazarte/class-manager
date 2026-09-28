namespace ClassManager.Api.I.Tests.Endpoints.Coaches.When_coach_lists_class_groups;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_their_class_groups_are_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_their_class_groups_are_returned_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();

        var classGroups = await scenario.Coach.ListClassGroupsAsync();

        classGroups!.Select(classGroup => classGroup.Id).ShouldBe([scenario.CoachClassGroup.Id]);
    }
}
