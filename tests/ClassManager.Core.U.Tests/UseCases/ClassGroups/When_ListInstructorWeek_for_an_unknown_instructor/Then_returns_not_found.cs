using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Core.U.Tests.UseCases.ClassGroups.When_ListInstructorWeek_for_an_unknown_instructor;

public sealed class Then_returns_not_found
{
    [Fact]
    public async Task Then_returns_not_found_Run()
    {
        var builder = new ClassGroupUseCaseBuilder();

        var response = await builder.BuildListInstructorWeek().ExecuteAsync(
            new ListInstructorWeekQuery(TestIds.Next()), CancellationToken.None);

        response.Error!.Kind.ShouldBe(ErrorKind.NotFound);
    }
}
