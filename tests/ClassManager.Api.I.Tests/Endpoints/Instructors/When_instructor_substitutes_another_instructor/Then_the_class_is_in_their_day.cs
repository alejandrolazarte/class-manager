namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_substitutes_another_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_class_is_in_their_day(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_class_is_in_their_day_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        using var assignment = await scenario.Business.HttpClient.PutSubstituteAsync(
            scenario.OtherClassGroup.Id, InstructorScenario.ClassDate, scenario.InstructorId);

        var sessions = await scenario.Instructor.GetDayAsync(InstructorScenario.ClassDate);

        sessions!.Select(session => session.ClassGroupName).ShouldBe([InstructorScenario.InstructorClassGroupName, InstructorScenario.OtherClassGroupName]);
        sessions![1].InstructorFullName.ShouldBe(InstructorScenario.InstructorFullName);
        sessions![1].OriginalInstructorFullName.ShouldBe(InstructorScenario.OtherInstructorFullName);
    }
}
