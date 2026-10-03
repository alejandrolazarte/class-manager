namespace ClassManager.Storage.I.Tests.Blobs.When_deleting_a_file_that_does_not_exist;

[Collection(AzuriteCollectionDefinition.Name)]
public sealed class Then_false_is_returned(AzuriteFixture fixture)
{
    [Fact]
    public async Task Then_false_is_returned_Run()
    {
        var storage = fixture.CreateStorage();
        await storage.Files.SaveAsync(FileSamples.PublicPngAt("photo.png"), CancellationToken.None);

        var deleted = await storage.Files.DeleteAsync("missing.png", FileVisibility.Public, CancellationToken.None);

        deleted.ShouldBeFalse();
    }
}
