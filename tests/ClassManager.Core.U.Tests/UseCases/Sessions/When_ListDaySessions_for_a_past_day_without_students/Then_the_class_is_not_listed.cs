using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_ListDaySessions_for_a_past_day_without_students;

public sealed class Then_the_class_is_not_listed
{
    [Fact]
    public async Task Then_the_class_is_not_listed_Run()
    {
        var builder = new SessionUseCaseBuilder();
        var lastWeek = TestData.Today.AddDays(-7);
        builder.ClassGroups.Setup(repository => repository.ListActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync([builder.ClassGroup]);
        builder.Enrollments.Setup(repository => repository.CountActiveOnByClassGroupAsync(lastWeek, It.IsAny<CancellationToken>())).ReturnsAsync(new Dictionary<Guid, int>());

        var response = await builder.BuildListDay().ExecuteAsync(new ListDaySessionsQuery(lastWeek), CancellationToken.None);

        response.Value!.ShouldBeEmpty();
    }
}
