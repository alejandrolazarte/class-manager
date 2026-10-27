namespace ClassManager.Api.I.Tests.Endpoints.Students.When_a_student_who_took_classes_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_todays_class_no_longer_lists_them(ApiFixture fixture)
{
    [Fact]
    public async Task Then_todays_class_no_longer_lists_them_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var today = EnrollmentRequests.Today;
        var instructor = await owner.CreateInstructorAsync();
        var classGroup = await owner.CreateClassGroupAsync(ClassGroupRequests.ClassGroupFor(instructor.Id, [today.DayOfWeek]));
        var studentId = await owner.RegisterStudentAsync();
        await owner.EnrollAsync(classGroup.Id, studentId, today.AddDays(-7));

        (await owner.DeleteStudentAsync(studentId)).EnsureSuccessStatusCode();

        (await owner.GetSessionAsync(classGroup.Id, today))!.Students.ShouldBeEmpty();
    }
}
