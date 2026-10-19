using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_RecordClassAttendance_has_no_students;

public sealed class Then_it_is_rejected
{
    [Fact]
    public async Task Then_it_is_rejected_Run()
    {
        var builder = new SessionUseCaseBuilder();

        var response = await builder.BuildRecordClass().ExecuteAsync(
            new RecordClassAttendanceCommand(builder.ClassGroup.Id, TestData.Today, []),
            CancellationToken.None);

        response.Error!.Kind.ShouldBe(ErrorKind.Validation);
    }
}
