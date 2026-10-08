namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_lists_class_groups;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_their_class_groups_are_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_their_class_groups_are_returned_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        var classGroups = await scenario.Instructor.ListClassGroupsAsync();

        classGroups!.Select(classGroup => classGroup.Id).ShouldBe([scenario.InstructorClassGroup.Id]);
    }
}
