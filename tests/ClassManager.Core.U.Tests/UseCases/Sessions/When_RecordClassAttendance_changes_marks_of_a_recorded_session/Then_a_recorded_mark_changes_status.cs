using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_RecordClassAttendance_changes_marks_of_a_recorded_session;

public sealed class Then_a_recorded_mark_changes_status
{
    [Fact]
    public async Task Then_a_recorded_mark_changes_status_Run()
    {
        var builder = new SessionUseCaseBuilder();
        var recordedSession = ClassSession.Create(builder.ClassGroup.Id, TestData.Today, TestData.Now).WithId();
        var recordedMark = Attendance.Create(recordedSession, builder.EnrolledStudentId, AttendanceStatus.Present);
        builder.RecordedSession(recordedMark);

        await builder.BuildRecordClass().ExecuteAsync(
            new RecordClassAttendanceCommand(builder.ClassGroup.Id, TestData.Today, [new StudentAttendance(builder.EnrolledStudentId, AttendanceStatus.Absent)]),
            CancellationToken.None);

        recordedMark.Status.ShouldBe(AttendanceStatus.Absent);
        builder.Attendances.Verify(repository => repository.Add(It.IsAny<Attendance>()), Times.Never);
    }
}
