namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPackClasses.When_student_books_a_pack_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_is_in_the_class(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_is_in_the_class_Run()
    {
        var scenario = await fixture.SeedPackStudentAppScenarioAsync();

        await scenario.BookPackClassAsync();

        var session = await scenario.Owner.GetSessionAsync(scenario.PackClassGroupId, InstructorScenario.ClassDate);
        session!.Students.Single(student => student.StudentId == scenario.StudentId).IsPackBooking.ShouldBeTrue();
        (await scenario.Student.GetPackClassesAsync(scenario.StudentId))!.ClassesLeft.ShouldBe(3);
    }
}
