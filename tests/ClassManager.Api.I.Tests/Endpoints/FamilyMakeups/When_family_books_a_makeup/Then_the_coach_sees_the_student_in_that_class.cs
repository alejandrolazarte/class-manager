namespace ClassManager.Api.I.Tests.Endpoints.FamilyMakeups.When_family_books_a_makeup;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_coach_sees_the_student_in_that_class(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_coach_sees_the_student_in_that_class_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        await scenario.NoticeAndBookOtherClassAsync();

        var session = await scenario.Coaches.Business.HttpClient.GetSessionAsync(scenario.Coaches.OtherClassGroup.Id, CoachScenario.ClassDate);

        session!.Students.Single(student => student.StudentId == scenario.Coaches.CoachStudentId).IsMakeup.ShouldBeTrue();
    }
}
