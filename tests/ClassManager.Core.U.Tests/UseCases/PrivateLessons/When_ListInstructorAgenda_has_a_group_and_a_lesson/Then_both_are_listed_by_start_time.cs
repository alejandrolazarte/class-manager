using ClassManager.Core.UseCases.PrivateLessons;

namespace ClassManager.Core.U.Tests.UseCases.PrivateLessons.When_ListInstructorAgenda_has_a_group_and_a_lesson;

public sealed class Then_both_are_listed_by_start_time
{
    [Fact]
    public async Task Then_both_are_listed_by_start_time_Run()
    {
        var builder = new PrivateLessonUseCaseBuilder();
        builder.InstructorTeachesGroupAt(builder.Instructor.Id, "18:30");
        builder.InstructorHasLessons(builder.ExistingLesson(TestData.Today, "10:00"));

        var response = await builder.BuildListAgenda().ExecuteAsync(
            new ListInstructorAgendaQuery(builder.Instructor.Id, TestData.Today), CancellationToken.None);

        response.Value!.Select(slot => (slot.StartTime, slot.Kind)).ShouldBe(
        [
            ("10:00", InstructorAgendaSlotKind.PrivateLesson),
            ("18:30", InstructorAgendaSlotKind.ClassGroup),
        ]);
    }
}
