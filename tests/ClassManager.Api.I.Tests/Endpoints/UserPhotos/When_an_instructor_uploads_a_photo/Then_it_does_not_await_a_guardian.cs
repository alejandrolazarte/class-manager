namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_an_instructor_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_does_not_await_a_guardian(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_does_not_await_a_guardian_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        await scenario.Instructor.ChangeMyPhotoAsync();

        var photo = await scenario.Instructor.GetMyPhotoAsync();
        photo!.AwaitsGuardian.ShouldBeFalse();
        photo.Url.ShouldNotBeNull();
    }
}
