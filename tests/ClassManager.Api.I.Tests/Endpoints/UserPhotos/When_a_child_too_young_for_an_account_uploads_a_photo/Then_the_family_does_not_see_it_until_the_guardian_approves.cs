namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_a_child_too_young_for_an_account_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_family_does_not_see_it_until_the_guardian_approves(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_family_does_not_see_it_until_the_guardian_approves_Run()
    {
        using var scenario = await fixture.SeedGuardedChildAsync();

        await scenario.Child.ChangeMyPhotoAsync();

        var family = await scenario.Guardian.GetStudentAppFamilyAsync();
        family!.Members.Single(person => !person.PaysForFamily).PhotoUrl.ShouldBeNull();
        await scenario.Guardian.PostPhotoApprovalAsync((await scenario.Guardian.ListPhotoApprovalsAsync()).Single().Id);
        (await scenario.Guardian.GetStudentAppFamilyAsync())!.Members.Single(person => !person.PaysForFamily).PhotoUrl.ShouldNotBeNull();
    }
}
