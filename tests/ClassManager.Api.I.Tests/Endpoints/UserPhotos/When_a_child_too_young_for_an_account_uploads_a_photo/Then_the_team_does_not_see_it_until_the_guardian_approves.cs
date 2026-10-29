namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_a_child_too_young_for_an_account_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_team_does_not_see_it_until_the_guardian_approves(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_team_does_not_see_it_until_the_guardian_approves_Run()
    {
        using var scenario = await fixture.SeedGuardedChildAsync();
        var team = scenario.Business.HttpClient;
        var instructor = await team.CreateInstructorAsync();
        var classGroup = await team.CreateClassGroupAsync(ClassGroupRequests.ClassGroupFor(instructor.Id, [EnrollmentRequests.Today.DayOfWeek]));
        await team.EnrollAsync(classGroup.Id, scenario.ChildStudentId, EnrollmentRequests.Today);

        await scenario.Child.ChangeMyPhotoAsync();

        var session = await team.GetSessionAsync(classGroup.Id, EnrollmentRequests.Today);
        session!.Students.Single().PhotoUrl.ShouldBeNull();
        await scenario.Guardian.PostPhotoApprovalAsync((await scenario.Guardian.ListPhotoApprovalsAsync()).Single().Id);
        (await team.GetSessionAsync(classGroup.Id, EnrollmentRequests.Today))!.Students.Single().PhotoUrl.ShouldNotBeNull();
    }
}
