namespace ClassManager.Storage.I.Tests.Blobs.When_deleting_a_saved_file;

[Collection(AzuriteCollectionDefinition.Name)]
public sealed class Then_its_url_no_longer_answers(AzuriteFixture fixture)
{
    [Fact]
    public async Task Then_its_url_no_longer_answers_Run()
    {
        var storage = fixture.CreateStorage();
        var fileUrl = await storage.SaveAsync(FileSamples.PngAt("tenant/products/photo.png"), CancellationToken.None);

        var deleted = await storage.DeleteAsync(fileUrl, CancellationToken.None);

        deleted.ShouldBeTrue();
        using var response = await fixture.AnonymousClient.GetAsync(fileUrl);
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
