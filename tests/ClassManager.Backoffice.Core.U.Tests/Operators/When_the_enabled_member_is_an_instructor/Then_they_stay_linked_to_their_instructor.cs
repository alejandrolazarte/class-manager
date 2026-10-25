using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Core.U.Tests.Operators.When_the_enabled_member_is_an_instructor;

public sealed class Then_they_stay_linked_to_their_instructor
{
    [Fact]
    public async Task Then_they_stay_linked_to_their_instructor_Run()
    {
        var scenario = new OperatorsScenario();
        var accessRequest = scenario.WithAccessRequest();
        var instructorId = TestIds.Next();
        var member = scenario.WithTeamMember("Persona", BusinessRole.BranchOwner, Guid.CreateVersion7(), instructorId);

        await scenario.EnableAsync(accessRequest.Id, member.Id, BusinessRole.PlatformOperator);

        member.InstructorId.ShouldBe(instructorId);
    }
}
