using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_RecordAttendance_for_student_not_enrolled;

public sealed class Then_returns_validation
{
    [Fact]
    public async Task Then_returns_validation_Run()
    {
        var builder = new SessionUseCaseBuilder();

        var response = await builder.BuildRecord().ExecuteAsync(
            new RecordAttendanceCommand(builder.ClassGroup.Id, TestData.Today, Guid.CreateVersion7(), AttendanceStatus.Present), CancellationToken.None);

        response.Error!.Code.ShouldBe(SessionErrorCodes.StudentNotEnrolled);
    }
}
