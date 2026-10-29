namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_an_instructor_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_team_sees_it_in_the_class(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_team_sees_it_in_the_class_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        await scenario.Instructor.ChangeMyPhotoAsync();

        var session = await scenario.Business.HttpClient.GetSessionAsync(scenario.InstructorClassGroup.Id, InstructorScenario.ClassDate);
        session!.InstructorPhotoUrl.ShouldNotBeNull();
    }
}
