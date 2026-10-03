namespace ClassManager.Storage.I.Tests.Blobs.When_deleting_a_file_with_spaces_in_its_path;

[Collection(AzuriteCollectionDefinition.Name)]
public sealed class Then_its_url_no_longer_answers(AzuriteFixture fixture)
{
    [Fact]
    public async Task Then_its_url_no_longer_answers_Run()
    {
        var storage = fixture.CreateStorage();
        var fileUrl = await storage.SaveAsync(FileSamples.PngAt("swim caps/blue cap.png"), CancellationToken.None);

        await storage.DeleteAsync(fileUrl, CancellationToken.None);

        using var response = await fixture.AnonymousClient.GetAsync(fileUrl);
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
