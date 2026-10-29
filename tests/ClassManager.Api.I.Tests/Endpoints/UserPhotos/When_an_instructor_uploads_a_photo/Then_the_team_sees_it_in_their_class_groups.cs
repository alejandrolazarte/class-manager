namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_an_instructor_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_team_sees_it_in_their_class_groups(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_team_sees_it_in_their_class_groups_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        await scenario.Instructor.ChangeMyPhotoAsync();

        var classGroups = await scenario.Business.HttpClient.ListClassGroupsAsync() ?? [];
        classGroups.Single(classGroup => classGroup.Id == scenario.InstructorClassGroup.Id).InstructorPhotoUrl.ShouldNotBeNull();
        classGroups.Single(classGroup => classGroup.Id == scenario.OtherClassGroup.Id).InstructorPhotoUrl.ShouldBeNull();
    }
}
