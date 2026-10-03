namespace ClassManager.Storage.I.Tests.Blobs.When_deleting_a_url_outside_the_container;

[Collection(AzuriteCollectionDefinition.Name)]
public sealed class Then_nothing_is_deleted(AzuriteFixture fixture)
{
    [Fact]
    public async Task Then_nothing_is_deleted_Run()
    {
        var ownStorage = fixture.CreateStorage();
        var otherStorage = fixture.CreateStorage();
        var otherFileUrl = await otherStorage.SaveAsync(FileSamples.PngAt("photo.png"), CancellationToken.None);

        var deleted = await ownStorage.DeleteAsync(otherFileUrl, CancellationToken.None);

        deleted.ShouldBeFalse();
        using var response = await fixture.AnonymousClient.GetAsync(otherFileUrl);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
