namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_a_child_too_young_for_an_account_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_child_cannot_approve_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_child_cannot_approve_it_Run()
    {
        using var scenario = await fixture.SeedGuardedChildAsync();
        await scenario.Child.ChangeMyPhotoAsync();
        var approval = (await scenario.Guardian.ListPhotoApprovalsAsync()).Single();

        using var response = await scenario.Child.PostPhotoApprovalAsync(approval.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        (await scenario.Child.GetMyPhotoAsync())!.AwaitsGuardian.ShouldBeTrue();
    }
}
