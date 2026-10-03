namespace ClassManager.Storage.I.Tests.Blobs.When_the_connection_string_is_missing;

[Collection(AzuriteCollectionDefinition.Name)]
public sealed class Then_saving_a_file_fails(AzuriteFixture fixture)
{
    [Fact]
    public async Task Then_saving_a_file_fails_Run()
    {
        var storage = fixture.CreateStorage(connectionString: string.Empty);

        await Should.ThrowAsync<InvalidOperationException>(() =>
            storage.Files.SaveAsync(FileSamples.PublicPngAt("photo.png"), CancellationToken.None));
    }
}
