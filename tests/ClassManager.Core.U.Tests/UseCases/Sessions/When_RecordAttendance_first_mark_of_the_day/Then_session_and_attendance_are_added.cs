using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_RecordAttendance_first_mark_of_the_day;

public sealed class Then_session_and_attendance_are_added
{
    [Fact]
    public async Task Then_session_and_attendance_are_added_Run()
    {
        var builder = new SessionUseCaseBuilder();

        var response = await builder.BuildRecord().ExecuteAsync(
            new RecordAttendanceCommand(builder.ClassGroup.Id, TestData.Today, builder.EnrolledStudentId, AttendanceStatus.Present), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        builder.Sessions.Verify(repository => repository.Add(It.Is<ClassSession>(session => session.Date == TestData.Today)), Times.Once);
        builder.Attendances.Verify(repository => repository.Add(It.Is<Attendance>(attendance => attendance.Status == AttendanceStatus.Present)), Times.Once);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
