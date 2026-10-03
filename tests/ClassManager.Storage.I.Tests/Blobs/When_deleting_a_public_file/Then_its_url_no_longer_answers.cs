namespace ClassManager.Storage.I.Tests.Blobs.When_deleting_a_public_file;

[Collection(AzuriteCollectionDefinition.Name)]
public sealed class Then_its_url_no_longer_answers(AzuriteFixture fixture)
{
    [Fact]
    public async Task Then_its_url_no_longer_answers_Run()
    {
        var storage = fixture.CreateStorage();
        const string path = "swim caps/blue cap.png";
        await storage.Files.SaveAsync(FileSamples.PublicPngAt(path), CancellationToken.None);

        var deleted = await storage.Files.DeleteAsync(path, FileVisibility.Public, CancellationToken.None);

        deleted.ShouldBeTrue();
        using var response = await fixture.AnonymousClient.GetAsync(storage.Files.PublicUrlOf(path));
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
