namespace ClassManager.Api.I.Tests.Endpoints.FamilyAbsences.When_family_takes_the_notice_back;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_coach_no_longer_sees_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_coach_no_longer_sees_it_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var classGroupId = scenario.Coaches.CoachClassGroup.Id;
        var studentId = scenario.Coaches.CoachStudentId;
        (await scenario.Family.PutAbsenceAsync(studentId, classGroupId, CoachScenario.ClassDate)).EnsureSuccessStatusCode();

        using var response = await scenario.Family.DeleteAbsenceAsync(studentId, classGroupId, CoachScenario.ClassDate);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var session = await scenario.Coaches.Coach.GetSessionAsync(classGroupId, CoachScenario.ClassDate);
        session!.Students.Single().AbsenceNotified.ShouldBeFalse();
    }
}
