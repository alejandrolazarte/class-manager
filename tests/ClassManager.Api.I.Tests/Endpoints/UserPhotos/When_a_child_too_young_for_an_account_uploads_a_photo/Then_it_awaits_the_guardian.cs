namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_a_child_too_young_for_an_account_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_awaits_the_guardian(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_awaits_the_guardian_Run()
    {
        using var scenario = await fixture.SeedGuardedChildAsync();

        var photo = await scenario.Child.ChangeMyPhotoAsync();

        photo.AwaitsGuardian.ShouldBeTrue();
        photo.NeedsGuardian.ShouldBeTrue();
    }
}
