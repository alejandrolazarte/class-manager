using ClassManager.Core.UseCases.PrivateLessons;

namespace ClassManager.Core.U.Tests.UseCases.PrivateLessons.When_ListInstructorAgenda_for_an_instructor_who_substitutes;

public sealed class Then_the_substituted_class_is_listed
{
    [Fact]
    public async Task Then_the_substituted_class_is_listed_Run()
    {
        var builder = new PrivateLessonUseCaseBuilder();
        var classGroup = builder.InstructorTeachesGroupAt(builder.Instructor.Id, "18:30");
        builder.SubstituteTodayIn(classGroup, builder.OtherInstructor.Id);

        var response = await builder.BuildListAgenda().ExecuteAsync(
            new ListInstructorAgendaQuery(builder.OtherInstructor.Id, TestData.Today), CancellationToken.None);

        response.Value!.ShouldHaveSingleItem().ClassGroupName.ShouldBe(classGroup.Name);
    }
}
