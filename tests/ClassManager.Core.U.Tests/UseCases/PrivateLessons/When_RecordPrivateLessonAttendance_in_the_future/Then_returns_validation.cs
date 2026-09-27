using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.PrivateLessons;

namespace ClassManager.Core.U.Tests.UseCases.PrivateLessons.When_RecordPrivateLessonAttendance_in_the_future;

public sealed class Then_returns_validation
{
    [Fact]
    public async Task Then_returns_validation_Run()
    {
        var builder = new PrivateLessonUseCaseBuilder();
        var tomorrowLesson = builder.ExistingLesson(TestData.Today.AddDays(1));
        builder.PrivateLessons.Setup(repository => repository.GetForUpdateAsync(tomorrowLesson.Id, It.IsAny<CancellationToken>())).ReturnsAsync(tomorrowLesson);

        var response = await builder.BuildRecordAttendance().ExecuteAsync(
            new RecordPrivateLessonAttendanceCommand(tomorrowLesson.Id, builder.StudentId, AttendanceStatus.Present), CancellationToken.None);

        response.Error!.Code.ShouldBe(SessionErrorCodes.InFuture);
    }
}
