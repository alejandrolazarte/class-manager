namespace ClassManager.Api.I.Tests.Endpoints.StudentAppMakeups.When_student_books_a_makeup;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_day_list_counts_the_student(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_day_list_counts_the_student_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        await scenario.NoticeAndBookOtherClassAsync();

        var day = await scenario.Instructors.Business.HttpClient.GetDayAsync(InstructorScenario.ClassDate);

        day!.Single(session => session.ClassGroupId == scenario.Instructors.OtherClassGroup.Id).EnrolledCount.ShouldBe(2);
    }
}
