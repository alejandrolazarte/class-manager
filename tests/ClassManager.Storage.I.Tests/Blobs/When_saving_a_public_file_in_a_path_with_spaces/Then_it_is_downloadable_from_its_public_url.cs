namespace ClassManager.Storage.I.Tests.Blobs.When_saving_a_public_file_in_a_path_with_spaces;

[Collection(AzuriteCollectionDefinition.Name)]
public sealed class Then_it_is_downloadable_from_its_public_url(AzuriteFixture fixture)
{
    [Fact]
    public async Task Then_it_is_downloadable_from_its_public_url_Run()
    {
        var storage = fixture.CreateStorage();
        const string path = "swim caps/blue cap.png";

        await storage.Files.SaveAsync(FileSamples.PublicPngAt(path), CancellationToken.None);

        using var response = await fixture.AnonymousClient.GetAsync(storage.Files.PublicUrlOf(path));
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
