using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_RecordClassAttendance_changes_marks_of_a_recorded_session;

public sealed class Then_the_recorded_marks_are_read_in_one_query
{
    [Fact]
    public async Task Then_the_recorded_marks_are_read_in_one_query_Run()
    {
        var builder = new SessionUseCaseBuilder();
        var anotherStudentId = builder.EnrollAnotherStudent();
        var session = builder.RecordedSession();

        var response = await builder.BuildRecordClass().ExecuteAsync(
            new RecordClassAttendanceCommand(
                builder.ClassGroup.Id,
                TestData.Today,
                [new StudentAttendance(builder.EnrolledStudentId, AttendanceStatus.Absent), new StudentAttendance(anotherStudentId, AttendanceStatus.Present)]),
            CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        builder.Attendances.Verify(repository => repository.ListBySessionForUpdateAsync(session.Id, It.IsAny<CancellationToken>()), Times.Once);
        builder.Attendances.Verify(
            repository => repository.FindForUpdateAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
