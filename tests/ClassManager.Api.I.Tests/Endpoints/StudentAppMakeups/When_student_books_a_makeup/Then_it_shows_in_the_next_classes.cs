namespace ClassManager.Api.I.Tests.Endpoints.StudentAppMakeups.When_student_books_a_makeup;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_shows_in_the_next_classes(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_shows_in_the_next_classes_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        await scenario.NoticeAndBookOtherClassAsync();

        var home = await scenario.Student.GetStudentAppHomeAsync();

        home!.Students.Single().NextClasses
            .ShouldContain(nextClass => nextClass.ClassGroupId == scenario.Instructors.OtherClassGroup.Id && nextClass.IsMakeup);
    }
}
