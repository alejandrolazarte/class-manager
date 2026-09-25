using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_RecordAttendance_in_the_future;

public sealed class Then_returns_validation
{
    [Fact]
    public async Task Then_returns_validation_Run()
    {
        var builder = new SessionUseCaseBuilder();
        var nextWeek = TestData.Today.AddDays(7);

        var response = await builder.BuildRecord().ExecuteAsync(
            new RecordAttendanceCommand(builder.ClassGroup.Id, nextWeek, builder.EnrolledStudentId, AttendanceStatus.Present), CancellationToken.None);

        response.Error!.Code.ShouldBe(SessionErrorCodes.InFuture);
    }
}
