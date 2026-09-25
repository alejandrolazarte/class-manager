using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_RecordAttendance_with_null_status;

public sealed class Then_mark_is_removed
{
    [Fact]
    public async Task Then_mark_is_removed_Run()
    {
        var builder = new SessionUseCaseBuilder();
        var session = ClassSession.Create(builder.ClassGroup.Id, TestData.Today, TestData.Now);
        var attendance = Attendance.Create(session.Id, builder.EnrolledStudentId, AttendanceStatus.Absent);
        builder.Sessions.Setup(repository => repository.FindForUpdateAsync(builder.ClassGroup.Id, TestData.Today, It.IsAny<CancellationToken>())).ReturnsAsync(session);
        builder.Attendances.Setup(repository => repository.FindForUpdateAsync(session.Id, builder.EnrolledStudentId, It.IsAny<CancellationToken>())).ReturnsAsync(attendance);

        await builder.BuildRecord().ExecuteAsync(
            new RecordAttendanceCommand(builder.ClassGroup.Id, TestData.Today, builder.EnrolledStudentId, null), CancellationToken.None);

        builder.Attendances.Verify(repository => repository.Remove(attendance), Times.Once);
    }
}
