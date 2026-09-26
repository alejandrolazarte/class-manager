using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.U.Tests.UseCases.Sessions.When_RescheduleSession_first_change_of_the_day;

public sealed class Then_session_is_added_with_new_time
{
    [Fact]
    public async Task Then_session_is_added_with_new_time_Run()
    {
        var builder = new SessionUseCaseBuilder();

        var response = await builder.BuildReschedule().ExecuteAsync(
            new RescheduleSessionCommand(builder.ClassGroup.Id, TestData.Today, "19:00"), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        builder.Sessions.Verify(
            repository => repository.Add(It.Is<ClassSession>(session => session.RescheduledStartTime == new TimeOnly(19, 0))),
            Times.Once);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
