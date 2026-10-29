namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_a_child_too_young_for_an_account_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_guardian_is_asked_to_approve_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_guardian_is_asked_to_approve_it_Run()
    {
        using var scenario = await fixture.SeedGuardedChildAsync();

        await scenario.Child.ChangeMyPhotoAsync();

        var approval = (await scenario.Guardian.ListPhotoApprovalsAsync()).ShouldHaveSingleItem();
        approval.StudentFullName.ShouldBe(StudentAppRequests.ChildFullName);
        using var download = await fixture.AnonymousClient.GetAsync(new Uri(approval.PhotoUrl));
        download.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
