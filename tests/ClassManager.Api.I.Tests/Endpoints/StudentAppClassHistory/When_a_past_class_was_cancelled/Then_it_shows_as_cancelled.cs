using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppClassHistory.When_a_past_class_was_cancelled;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_shows_as_cancelled(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_shows_as_cancelled_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var today = InstructorScenario.ClassDate;
        var weekly = await scenario.EnrollInWeeklyClassSinceAsync(today.AddDays(-7));
        (await scenario.Instructors.Business.HttpClient.PutSessionCancellationAsync(weekly.Id, today.AddDays(-7), "Feriado")).EnsureSuccessStatusCode();

        var history = await scenario.Student.GetClassHistoryAsync(scenario.Instructors.InstructorStudentId, ClassHistoryRequests.CurrentMonth);

        history!.Classes.Single().Status.ShouldBe(PastClassStatus.Cancelled);
    }
}
