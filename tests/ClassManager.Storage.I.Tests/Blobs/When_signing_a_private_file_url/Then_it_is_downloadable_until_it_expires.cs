namespace ClassManager.Storage.I.Tests.Blobs.When_signing_a_private_file_url;

[Collection(AzuriteCollectionDefinition.Name)]
public sealed class Then_it_is_downloadable_until_it_expires(AzuriteFixture fixture)
{
    [Fact]
    public async Task Then_it_is_downloadable_until_it_expires_Run()
    {
        var storage = fixture.CreateStorage();
        const string path = "tenant/users/1/photo.png";
        await storage.Files.SaveAsync(FileSamples.PrivatePngAt(path), CancellationToken.None);

        var signedUrl = storage.Files.PrivateReadUrlOf(path, DateTimeOffset.UtcNow.AddMinutes(10));

        using var response = await fixture.AnonymousClient.GetAsync(signedUrl);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsByteArrayAsync()).ShouldBe(FileSamples.Png);
    }
}
