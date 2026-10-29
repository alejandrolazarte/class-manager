namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_the_guardian_approves_the_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_guardian_sees_it_on_the_home(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_guardian_sees_it_on_the_home_Run()
    {
        using var scenario = await fixture.SeedGuardedChildAsync();
        await scenario.Child.ChangeMyPhotoAsync();
        var approval = (await scenario.Guardian.ListPhotoApprovalsAsync()).Single();

        await scenario.Guardian.PostPhotoApprovalAsync(approval.Id);

        var home = await scenario.Guardian.GetStudentAppHomeAsync();
        home!.Students.Single(student => student.Id == scenario.ChildStudentId).PhotoUrl.ShouldNotBeNull();
    }
}
