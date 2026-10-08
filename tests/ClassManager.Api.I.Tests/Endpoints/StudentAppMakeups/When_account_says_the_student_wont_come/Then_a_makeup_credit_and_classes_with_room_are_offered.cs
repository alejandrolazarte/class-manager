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
        var instructors = scenario.Instructors;
        (await scenario.Student.PutAbsenceAsync(instructors.InstructorStudentId, instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate)).EnsureSuccessStatusCode();

        var makeups = await scenario.Student.GetMakeupsAsync(instructors.InstructorStudentId);

        makeups!.Credits.ShouldHaveSingleItem().ShouldBe(new StudentAppMakeupCreditResponse(
            InstructorScenario.ClassDate, MakeupReason.Notice, InstructorScenario.ClassDate.AddDays(MakeupCredits.ValidityDays)));
        var slot = makeups.Slots.Where(slot => slot.Date == InstructorScenario.ClassDate).ShouldHaveSingleItem();
        slot.ClassGroupId.ShouldBe(instructors.OtherClassGroup.Id);
        slot.SpotsLeft.ShouldBe(instructors.OtherClassGroup.Capacity - 1);
        slot.IsBooked.ShouldBeFalse();
    }
}
