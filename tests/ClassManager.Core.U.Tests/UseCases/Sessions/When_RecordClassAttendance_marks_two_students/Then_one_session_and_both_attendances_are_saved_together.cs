using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_RecordClassAttendance_marks_two_students;

public sealed class Then_one_session_and_both_attendances_are_saved_together
{
    [Fact]
    public async Task Then_one_session_and_both_attendances_are_saved_together_Run()
    {
        var builder = new SessionUseCaseBuilder();
        var anotherStudentId = builder.EnrollAnotherStudent();

        var response = await builder.BuildRecordClass().ExecuteAsync(
            new RecordClassAttendanceCommand(
                builder.ClassGroup.Id,
                TestData.Today,
                [new StudentAttendance(builder.EnrolledStudentId, AttendanceStatus.Present), new StudentAttendance(anotherStudentId, AttendanceStatus.Present)]),
            CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        builder.Sessions.Verify(repository => repository.Add(It.Is<ClassSession>(session => session.Date == TestData.Today)), Times.Once);
        builder.Attendances.Verify(repository => repository.Add(It.Is<Attendance>(attendance => attendance.Status == AttendanceStatus.Present)), Times.Exactly(2));
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
