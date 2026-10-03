namespace ClassManager.Storage.I.Tests.Blobs.When_saving_a_private_file;

[Collection(AzuriteCollectionDefinition.Name)]
public sealed class Then_it_cannot_be_downloaded_anonymously(AzuriteFixture fixture)
{
    [Fact]
    public async Task Then_it_cannot_be_downloaded_anonymously_Run()
    {
        var storage = fixture.CreateStorage();
        const string path = "tenant/certificates/medical.png";

        await storage.Files.SaveAsync(FileSamples.PrivatePngAt(path), CancellationToken.None);

        (await storage.PrivateFileExistsAsync(path)).ShouldBeTrue();
        using var response = await fixture.AnonymousClient.GetAsync(storage.PrivateUrlOf(path));
        response.IsSuccessStatusCode.ShouldBeFalse();
    }
}
