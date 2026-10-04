namespace ClassManager.Api.I.Tests.Endpoints.StudentAppMakeups.When_student_cancels_a_makeup;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_credit_is_back(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_credit_is_back_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        await scenario.NoticeAndBookOtherClassAsync();

        using var response = await scenario.Student.DeleteMakeupAsync(
            scenario.Coaches.CoachStudentId, scenario.Coaches.OtherClassGroup.Id, CoachScenario.ClassDate);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await scenario.Student.GetMakeupsAsync(scenario.Coaches.CoachStudentId))!.Credits.ShouldHaveSingleItem();
    }
}
