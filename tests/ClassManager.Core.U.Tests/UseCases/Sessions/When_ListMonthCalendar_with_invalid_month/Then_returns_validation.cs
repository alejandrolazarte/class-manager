using ClassManager.Core.Common;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_ListMonthCalendar_with_invalid_month;

public sealed class Then_returns_validation
{
    [Fact]
    public async Task Then_returns_validation_Run()
    {
        var builder = new SessionUseCaseBuilder();

        var response = await builder.BuildMonthCalendar().ExecuteAsync(new ListMonthCalendarQuery("septiembre"), CancellationToken.None);

        response.Error!.Kind.ShouldBe(ErrorKind.Validation);
    }
}
