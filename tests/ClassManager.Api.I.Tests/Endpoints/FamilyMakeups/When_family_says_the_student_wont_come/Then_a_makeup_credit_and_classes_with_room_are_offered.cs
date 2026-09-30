using ClassManager.Core.Domain.Makeups;
using ClassManager.Core.UseCases.Families;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyMakeups.When_family_says_the_student_wont_come;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_makeup_credit_and_classes_with_room_are_offered(ApiFixture fixture)
{
    [Fact]
    public async Task Then_a_makeup_credit_and_classes_with_room_are_offered_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var coaches = scenario.Coaches;
        (await scenario.Family.PutAbsenceAsync(coaches.CoachStudentId, coaches.CoachClassGroup.Id, CoachScenario.ClassDate)).EnsureSuccessStatusCode();

        var makeups = await scenario.Family.GetMakeupsAsync(coaches.CoachStudentId);

        makeups!.Credits.ShouldHaveSingleItem().ShouldBe(new FamilyMakeupCreditResponse(
            CoachScenario.ClassDate, MakeupReason.Notice, CoachScenario.ClassDate.AddDays(MakeupCredits.ValidityDays)));
        var slot = makeups.Slots.Where(slot => slot.Date == CoachScenario.ClassDate).ShouldHaveSingleItem();
        slot.ClassGroupId.ShouldBe(coaches.OtherClassGroup.Id);
        slot.SpotsLeft.ShouldBe(coaches.OtherClassGroup.Capacity - 1);
        slot.IsBooked.ShouldBeFalse();
    }
}
