namespace ClassManager.Storage.I.Tests.Blobs.When_a_signed_private_file_url_has_expired;

[Collection(AzuriteCollectionDefinition.Name)]
public sealed class Then_it_no_longer_answers(AzuriteFixture fixture)
{
    [Fact]
    public async Task Then_it_no_longer_answers_Run()
    {
        var storage = fixture.CreateStorage();
        const string path = "tenant/users/1/photo.png";
        await storage.Files.SaveAsync(FileSamples.PrivatePngAt(path), CancellationToken.None);

        var signedUrl = storage.Files.PrivateReadUrlOf(path, DateTimeOffset.UtcNow.AddMinutes(-1));

        using var response = await fixture.AnonymousClient.GetAsync(signedUrl);
        response.IsSuccessStatusCode.ShouldBeFalse();
    }
}
