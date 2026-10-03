namespace ClassManager.Storage.I.Tests.Blobs.When_saving_a_public_file;

[Collection(AzuriteCollectionDefinition.Name)]
public sealed class Then_it_can_be_cached_by_browsers(AzuriteFixture fixture)
{
    [Fact]
    public async Task Then_it_can_be_cached_by_browsers_Run()
    {
        var storage = fixture.CreateStorage();
        const string path = "photo.png";

        await storage.Files.SaveAsync(FileSamples.PublicPngAt(path), CancellationToken.None);

        using var response = await fixture.AnonymousClient.GetAsync(storage.Files.PublicUrlOf(path));
        response.Headers.CacheControl!.Public.ShouldBeTrue();
    }
}
