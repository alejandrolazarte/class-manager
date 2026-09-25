using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Core.U.Tests.UseCases.ClassGroups.When_UpdateClassGroup_keeps_its_own_time;

public sealed class Then_it_does_not_conflict_with_itself
{
    [Fact]
    public async Task Then_it_does_not_conflict_with_itself_Run()
    {
        var builder = new ClassGroupUseCaseBuilder();
        var classGroup = builder.ExistingClassGroup();
        builder.ClassGroups.Setup(repository => repository.GetForUpdateAsync(classGroup.Id, It.IsAny<CancellationToken>())).ReturnsAsync(classGroup);
        builder.ClassGroups
            .Setup(repository => repository.ListActiveByInstructorAsync(builder.Instructor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([classGroup]);
        var details = builder.ValidDetails() with { Weekdays = [DayOfWeek.Thursday] };

        var response = await builder.BuildUpdate().ExecuteAsync(new UpdateClassGroupCommand(classGroup.Id, details), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
    }
}
