namespace ClassManager.Storage.I.Tests.Blobs.When_saving_a_file;

[Collection(AzuriteCollectionDefinition.Name)]
public sealed class Then_it_is_downloadable_from_its_url(AzuriteFixture fixture)
{
    [Fact]
    public async Task Then_it_is_downloadable_from_its_url_Run()
    {
        var storage = fixture.CreateStorage();

        var fileUrl = await storage.SaveAsync(FileSamples.PngAt("tenant/products/photo.png"), CancellationToken.None);

        using var response = await fixture.AnonymousClient.GetAsync(fileUrl);
        (await response.Content.ReadAsByteArrayAsync()).ShouldBe(FileSamples.Png);
        response.Content.Headers.ContentType!.MediaType.ShouldBe(FileSamples.PngContentType);
    }
}
