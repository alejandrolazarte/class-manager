namespace ClassManager.Core.U.Tests.UseCases.Students.When_DeleteStudent_with_an_upcoming_pack_class;

public sealed class Then_the_booking_is_cancelled
{
    [Fact]
    public async Task Then_the_booking_is_cancelled_Run()
    {
        var builder = new DeleteStudentUseCaseBuilder();
        var booking = builder.WithUpcomingPackClass();

        await builder.Build().ExecuteAsync(builder.Command(), CancellationToken.None);

        builder.PackBookings.Verify(repository => repository.Remove(booking), Times.Once);
    }
}
