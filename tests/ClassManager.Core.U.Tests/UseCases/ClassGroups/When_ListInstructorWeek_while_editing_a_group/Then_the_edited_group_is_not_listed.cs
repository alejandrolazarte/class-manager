using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Core.U.Tests.UseCases.ClassGroups.When_ListInstructorWeek_while_editing_a_group;

public sealed class Then_the_edited_group_is_not_listed
{
    [Fact]
    public async Task Then_the_edited_group_is_not_listed_Run()
    {
        var builder = new ClassGroupUseCaseBuilder();
        var editedClassGroup = builder.ExistingClassGroup("18:00");
        builder.InstructorHasClassGroups(editedClassGroup);

        var response = await builder.BuildListInstructorWeek().ExecuteAsync(
            new ListInstructorWeekQuery(builder.Instructor.Id, editedClassGroup.Id), CancellationToken.None);

        response.Value!.ShouldBeEmpty();
    }
}
