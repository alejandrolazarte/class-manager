using ClassManager.Core.UseCases.ClassGroups;
using ClassManager.Core.UseCases.PrivateLessons;

namespace ClassManager.Core.U.Tests.UseCases.ClassGroups.When_ListInstructorWeek_has_a_group_and_a_lesson;

public sealed class Then_each_is_listed_on_its_weekdays
{
    [Fact]
    public async Task Then_each_is_listed_on_its_weekdays_Run()
    {
        var builder = new ClassGroupUseCaseBuilder();
        var classGroup = builder.ExistingClassGroup("18:00");
        builder.InstructorHasClassGroups(classGroup);
        builder.InstructorHasUpcomingLessons(builder.ExistingLesson(TestData.Today.AddDays(1), "10:00"));

        var response = await builder.BuildListInstructorWeek().ExecuteAsync(
            new ListInstructorWeekQuery(builder.Instructor.Id), CancellationToken.None);

        response.Value!.ShouldBe(
        [
            new InstructorWeekSlotResponse(TestData.Today.DayOfWeek, "18:00", 60, InstructorAgendaSlotKind.ClassGroup, classGroup.Name),
            new InstructorWeekSlotResponse(TestData.Today.AddDays(1).DayOfWeek, "10:00", 45, InstructorAgendaSlotKind.PrivateLesson, null),
        ]);
    }
}
