namespace ClassManager.Storage.I.Tests.Blobs.When_saving_a_file_in_a_path_with_spaces;

[Collection(AzuriteCollectionDefinition.Name)]
public sealed class Then_it_is_downloadable_from_its_url(AzuriteFixture fixture)
{
    [Fact]
    public async Task Then_it_is_downloadable_from_its_url_Run()
    {
        var storage = fixture.CreateStorage();

        var fileUrl = await storage.SaveAsync(FileSamples.PngAt("swim caps/blue cap.png"), CancellationToken.None);

        using var response = await fixture.AnonymousClient.GetAsync(fileUrl);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
