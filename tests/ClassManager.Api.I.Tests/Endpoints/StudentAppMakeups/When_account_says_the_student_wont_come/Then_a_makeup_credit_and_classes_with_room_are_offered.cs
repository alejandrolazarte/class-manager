using ClassManager.Core.Domain.Makeups;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppMakeups.When_account_says_the_student_wont_come;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_makeup_credit_and_classes_with_room_are_offered(ApiFixture fixture)
{
    [Fact]
    public async Task Then_a_makeup_credit_and_classes_with_room_are_offered_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var coaches = scenario.Coaches;
        (await scenario.Student.PutAbsenceAsync(coaches.CoachStudentId, coaches.CoachClassGroup.Id, CoachScenario.ClassDate)).EnsureSuccessStatusCode();

        var makeups = await scenario.Student.GetMakeupsAsync(coaches.CoachStudentId);

        makeups!.Credits.ShouldHaveSingleItem().ShouldBe(new StudentAppMakeupCreditResponse(
            CoachScenario.ClassDate, MakeupReason.Notice, CoachScenario.ClassDate.AddDays(MakeupCredits.ValidityDays)));
        var slot = makeups.Slots.Where(slot => slot.Date == CoachScenario.ClassDate).ShouldHaveSingleItem();
        slot.ClassGroupId.ShouldBe(coaches.OtherClassGroup.Id);
        slot.SpotsLeft.ShouldBe(coaches.OtherClassGroup.Capacity - 1);
        slot.IsBooked.ShouldBeFalse();
    }
}
