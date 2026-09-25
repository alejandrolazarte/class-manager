using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_CancelSession_with_attendance;

public sealed class Then_returns_conflict
{
    [Fact]
    public async Task Then_returns_conflict_Run()
    {
        var builder = new SessionUseCaseBuilder();
        var session = ClassSession.Create(builder.ClassGroup.Id, TestData.Today, TestData.Now);
        builder.Sessions.Setup(repository => repository.FindForUpdateAsync(builder.ClassGroup.Id, TestData.Today, It.IsAny<CancellationToken>())).ReturnsAsync(session);
        builder.Attendances
            .Setup(repository => repository.ListBySessionAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([Attendance.Create(session.Id, builder.EnrolledStudentId, AttendanceStatus.Present)]);

        var response = await builder.BuildCancel().ExecuteAsync(
            new CancelSessionCommand(builder.ClassGroup.Id, TestData.Today, "Feriado"), CancellationToken.None);

        response.Error!.Code.ShouldBe(SessionErrorCodes.HasAttendance);
        session.IsCancelled.ShouldBeFalse();
    }
}
