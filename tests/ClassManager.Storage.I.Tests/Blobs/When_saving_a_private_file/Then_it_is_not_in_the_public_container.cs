namespace ClassManager.Storage.I.Tests.Blobs.When_saving_a_private_file;

[Collection(AzuriteCollectionDefinition.Name)]
public sealed class Then_it_is_not_in_the_public_container(AzuriteFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_in_the_public_container_Run()
    {
        var storage = fixture.CreateStorage();
        const string path = "tenant/certificates/medical.png";
        await storage.Files.SaveAsync(FileSamples.PublicPngAt("tenant/products/photo.png"), CancellationToken.None);

        await storage.Files.SaveAsync(FileSamples.PrivatePngAt(path), CancellationToken.None);

        using var response = await fixture.AnonymousClient.GetAsync(storage.Files.PublicUrlOf(path));
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
