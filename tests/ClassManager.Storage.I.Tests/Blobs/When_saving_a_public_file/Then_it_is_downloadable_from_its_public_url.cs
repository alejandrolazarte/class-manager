namespace ClassManager.Storage.I.Tests.Blobs.When_saving_a_public_file;

[Collection(AzuriteCollectionDefinition.Name)]
public sealed class Then_it_is_downloadable_from_its_public_url(AzuriteFixture fixture)
{
    [Fact]
    public async Task Then_it_is_downloadable_from_its_public_url_Run()
    {
        var storage = fixture.CreateStorage();
        const string path = "tenant/products/photo.png";

        await storage.Files.SaveAsync(FileSamples.PublicPngAt(path), CancellationToken.None);

        using var response = await fixture.AnonymousClient.GetAsync(storage.Files.PublicUrlOf(path));
        (await response.Content.ReadAsByteArrayAsync()).ShouldBe(FileSamples.Png);
        response.Content.Headers.ContentType!.MediaType.ShouldBe(FileSamples.PngContentType);
    }
}
