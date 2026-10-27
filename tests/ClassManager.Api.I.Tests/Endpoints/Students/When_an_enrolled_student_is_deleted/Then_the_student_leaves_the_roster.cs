namespace ClassManager.Api.I.Tests.Endpoints.Students.When_an_enrolled_student_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_leaves_the_roster(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_leaves_the_roster_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var studentId = await owner.RegisterStudentAsync();
        var classGroup = await owner.CreateClassGroupWithInstructorAsync();
        await owner.EnrollAsync(classGroup.Id, studentId, EnrollmentRequests.Today.AddMonths(-1));

        using var response = await owner.DeleteStudentAsync(studentId);

        (await owner.GetRosterAsync(classGroup.Id))!.ShouldBeEmpty();
    }
}
