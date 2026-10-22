using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_RecordClassAttendance_includes_a_student_not_in_the_class;

public sealed class Then_nothing_is_saved
{
    [Fact]
    public async Task Then_nothing_is_saved_Run()
    {
        var builder = new SessionUseCaseBuilder();

        var response = await builder.BuildRecordClass().ExecuteAsync(
            new RecordClassAttendanceCommand(
                builder.ClassGroup.Id,
                TestData.Today,
                [new StudentAttendance(builder.EnrolledStudentId, AttendanceStatus.Present), new StudentAttendance(TestIds.Next(), AttendanceStatus.Present)]),
            CancellationToken.None);

        response.Error!.Code.ShouldBe(SessionErrorCodes.StudentNotEnrolled);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
