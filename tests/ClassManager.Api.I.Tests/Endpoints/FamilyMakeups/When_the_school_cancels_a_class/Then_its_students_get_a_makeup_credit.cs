using ClassManager.Core.Domain.Makeups;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyMakeups.When_the_school_cancels_a_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_students_get_a_makeup_credit(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_students_get_a_makeup_credit_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var coaches = scenario.Coaches;
        (await coaches.Business.HttpClient.PutSessionCancellationAsync(coaches.CoachClassGroup.Id, CoachScenario.ClassDate, "Feriado"))
            .EnsureSuccessStatusCode();

        var makeups = await scenario.Family.GetMakeupsAsync(coaches.CoachStudentId);

        makeups!.Credits.ShouldHaveSingleItem().Reason.ShouldBe(MakeupReason.Cancelled);
    }
}
