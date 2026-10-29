namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_the_guardian_refuses_the_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_removed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_removed_Run()
    {
        using var scenario = await fixture.SeedGuardedChildAsync();
        var photo = await scenario.Child.ChangeMyPhotoAsync();
        var approval = (await scenario.Guardian.ListPhotoApprovalsAsync()).Single();

        using var response = await scenario.Guardian.PostPhotoRefusalAsync(approval.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await scenario.Child.GetMyPhotoAsync())!.Url.ShouldBeNull();
        using var download = await fixture.AnonymousClient.GetAsync(new Uri(photo.Url!));
        download.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
