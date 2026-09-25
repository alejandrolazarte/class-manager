using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_RecordAttendance_on_cancelled_session;

public sealed class Then_returns_conflict
{
    [Fact]
    public async Task Then_returns_conflict_Run()
    {
        var builder = new SessionUseCaseBuilder();
        builder.CancelledSession();

        var response = await builder.BuildRecord().ExecuteAsync(
            new RecordAttendanceCommand(builder.ClassGroup.Id, TestData.Today, builder.EnrolledStudentId, AttendanceStatus.Present), CancellationToken.None);

        response.Error!.Code.ShouldBe(SessionErrorCodes.Cancelled);
    }
}
