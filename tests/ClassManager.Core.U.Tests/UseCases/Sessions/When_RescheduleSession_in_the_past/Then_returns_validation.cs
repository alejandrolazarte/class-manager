using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_RescheduleSession_in_the_past;

public sealed class Then_returns_validation
{
    [Fact]
    public async Task Then_returns_validation_Run()
    {
        var builder = new SessionUseCaseBuilder();
        var lastWeek = TestData.Today.AddDays(-7);

        var response = await builder.BuildReschedule().ExecuteAsync(
            new RescheduleSessionCommand(builder.ClassGroup.Id, lastWeek, "19:00"), CancellationToken.None);

        response.Error!.Code.ShouldBe(SessionErrorCodes.InPast);
    }
}
