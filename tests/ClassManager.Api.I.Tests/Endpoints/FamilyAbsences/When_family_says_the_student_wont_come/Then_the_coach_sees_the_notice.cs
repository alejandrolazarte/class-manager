namespace ClassManager.Api.I.Tests.Endpoints.FamilyAbsences.When_family_says_the_student_wont_come;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_coach_sees_the_notice(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_coach_sees_the_notice_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var classGroupId = scenario.Coaches.CoachClassGroup.Id;
        var nextWeek = CoachScenario.ClassDate.AddDays(7);

        using var response = await scenario.Family.PutAbsenceAsync(scenario.Coaches.CoachStudentId, classGroupId, nextWeek);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var session = await scenario.Coaches.Coach.GetSessionAsync(classGroupId, nextWeek);
        session!.Students.Single().AbsenceNotified.ShouldBeTrue();
        var home = await scenario.Family.GetFamilyHomeAsync();
        home!.Students.Single().NextClasses.Single(nextClass => nextClass.Date == nextWeek).AbsenceNotified.ShouldBeTrue();
    }
}
