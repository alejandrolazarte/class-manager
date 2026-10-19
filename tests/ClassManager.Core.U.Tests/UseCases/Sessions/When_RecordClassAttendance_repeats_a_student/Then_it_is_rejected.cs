using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_RecordClassAttendance_repeats_a_student;

public sealed class Then_it_is_rejected
{
    [Fact]
    public async Task Then_it_is_rejected_Run()
    {
        var builder = new SessionUseCaseBuilder();

        var response = await builder.BuildRecordClass().ExecuteAsync(
            new RecordClassAttendanceCommand(
                builder.ClassGroup.Id,
                TestData.Today,
                [new StudentAttendance(builder.EnrolledStudentId, AttendanceStatus.Present), new StudentAttendance(builder.EnrolledStudentId, AttendanceStatus.Absent)]),
            CancellationToken.None);

        response.Error!.Kind.ShouldBe(ErrorKind.Validation);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
