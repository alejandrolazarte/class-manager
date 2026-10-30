namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_the_client_who_pays_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_team_sees_it_as_the_client_avatar(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_team_sees_it_as_the_client_avatar_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        await scenario.Student.ChangeMyPhotoAsync();

        var client = await scenario.Instructors.Business.HttpClient.GetClientAsync(scenario.ClientId);
        client.PhotoUrl.ShouldNotBeNull();
    }
}
