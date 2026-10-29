namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_an_instructor_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_cannot_be_downloaded_without_its_signature(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_cannot_be_downloaded_without_its_signature_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        var photo = await scenario.Instructor.ChangeMyPhotoAsync();

        var signedUrl = new Uri(photo.Url!);
        using var response = await fixture.AnonymousClient.GetAsync(new Uri(signedUrl.GetLeftPart(UriPartial.Path)));
        response.IsSuccessStatusCode.ShouldBeFalse();
    }
}
