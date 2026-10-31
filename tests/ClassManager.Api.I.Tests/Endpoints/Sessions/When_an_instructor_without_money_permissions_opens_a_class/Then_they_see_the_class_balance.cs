using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_an_instructor_without_money_permissions_opens_a_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_see_the_class_balance(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_see_the_class_balance_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        var session = await scenario.Instructor.GetSessionAsync(scenario.InstructorClassGroup.Id, InstructorScenario.ClassDate);

        session!.Students.Single().ClassBalance.ShouldBe(new SessionClassBalanceResponse(AvailableClasses: 0, UnpaidClasses: 0));
    }
}
