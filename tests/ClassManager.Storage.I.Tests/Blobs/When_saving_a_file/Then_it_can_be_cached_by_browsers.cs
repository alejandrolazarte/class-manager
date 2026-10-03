namespace ClassManager.Storage.I.Tests.Blobs.When_saving_a_file;

[Collection(AzuriteCollectionDefinition.Name)]
public sealed class Then_it_can_be_cached_by_browsers(AzuriteFixture fixture)
{
    [Fact]
    public async Task Then_it_can_be_cached_by_browsers_Run()
    {
        var storage = fixture.CreateStorage();

        var fileUrl = await storage.SaveAsync(FileSamples.PngAt("photo.png"), CancellationToken.None);

        using var response = await fixture.AnonymousClient.GetAsync(fileUrl);
        response.Headers.CacheControl!.Public.ShouldBeTrue();
    }
}
