using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPackClasses.When_student_books_a_full_pack_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var scenario = await fixture.SeedPackStudentAppScenarioAsync();
        var fullClassGroup = await scenario.Owner.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(scenario.Coaches.OtherInstructorId, [CoachScenario.ClassDate.DayOfWeek], "20:00") with { Name = "Natación llena", Capacity = 1 });
        await scenario.Owner.EnrollAsync(fullClassGroup.Id, scenario.Coaches.OtherStudentId, CoachScenario.ClassDate);
        var fullPack = await scenario.Owner.CreateClassPackAsync("Pack llena", 4, [fullClassGroup.Id]);
        await scenario.Owner.SellClassPackAsync(scenario.Scenario.ClientId, fullPack.Id);

        using var response = await scenario.Student.PutPackClassAsync(scenario.StudentId, fullClassGroup.Id, CoachScenario.ClassDate);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await response.Content.ReadAsStringAsync()).ShouldContain(SessionErrorCodes.MakeupFull);
    }
}
