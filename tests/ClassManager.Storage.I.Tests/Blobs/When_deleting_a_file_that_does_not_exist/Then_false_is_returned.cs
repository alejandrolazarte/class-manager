namespace ClassManager.Storage.I.Tests.Blobs.When_deleting_a_file_that_does_not_exist;

[Collection(AzuriteCollectionDefinition.Name)]
public sealed class Then_false_is_returned(AzuriteFixture fixture)
{
    [Fact]
    public async Task Then_false_is_returned_Run()
    {
        var storage = fixture.CreateStorage();
        var fileUrl = await storage.SaveAsync(FileSamples.PngAt("photo.png"), CancellationToken.None);
        var missingFileUrl = new Uri(fileUrl, "missing.png");

        var deleted = await storage.DeleteAsync(missingFileUrl, CancellationToken.None);

        deleted.ShouldBeFalse();
    }
}
