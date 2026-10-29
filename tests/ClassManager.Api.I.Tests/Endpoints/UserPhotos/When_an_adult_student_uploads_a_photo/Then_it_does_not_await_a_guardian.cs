namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_an_adult_student_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_does_not_await_a_guardian(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_does_not_await_a_guardian_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        var photo = await scenario.Student.ChangeMyPhotoAsync();

        photo.AwaitsGuardian.ShouldBeFalse();
        photo.NeedsGuardian.ShouldBeFalse();
    }
}
