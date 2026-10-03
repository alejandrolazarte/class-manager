namespace ClassManager.Storage.I.Tests.Blobs.When_deleting_a_private_file;

[Collection(AzuriteCollectionDefinition.Name)]
public sealed class Then_it_is_removed(AzuriteFixture fixture)
{
    [Fact]
    public async Task Then_it_is_removed_Run()
    {
        var storage = fixture.CreateStorage();
        const string path = "tenant/certificates/medical.png";
        await storage.Files.SaveAsync(FileSamples.PrivatePngAt(path), CancellationToken.None);

        var deleted = await storage.Files.DeleteAsync(path, FileVisibility.Private, CancellationToken.None);

        deleted.ShouldBeTrue();
        (await storage.PrivateFileExistsAsync(path)).ShouldBeFalse();
    }
}
