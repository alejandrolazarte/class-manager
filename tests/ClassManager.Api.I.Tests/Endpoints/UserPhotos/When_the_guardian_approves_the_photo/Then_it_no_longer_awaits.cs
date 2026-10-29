namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_the_guardian_approves_the_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_no_longer_awaits(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_no_longer_awaits_Run()
    {
        using var scenario = await fixture.SeedGuardedChildAsync();
        await scenario.Child.ChangeMyPhotoAsync();
        var approval = (await scenario.Guardian.ListPhotoApprovalsAsync()).Single();

        using var response = await scenario.Guardian.PostPhotoApprovalAsync(approval.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await scenario.Child.GetMyPhotoAsync())!.AwaitsGuardian.ShouldBeFalse();
        (await scenario.Guardian.ListPhotoApprovalsAsync()).ShouldBeEmpty();
    }
}
