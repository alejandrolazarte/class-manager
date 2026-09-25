using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Enrollments;
using ClassManager.Core.UseCases.Enrollments;

namespace ClassManager.Core.U.Tests.UseCases.Enrollments.When_EndEnrollment_before_start;

public sealed class Then_enrollment_is_removed
{
    [Fact]
    public async Task Then_enrollment_is_removed_Run()
    {
        var builder = new EnrollmentUseCaseBuilder();
        var futureEnrollment = builder.ExistingEnrollment(TestData.Today.AddDays(7));
        builder.Enrollments.Setup(repository => repository.GetForUpdateAsync(futureEnrollment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(futureEnrollment);

        var response = await builder.BuildEnd().ExecuteAsync(new EndEnrollmentCommand(futureEnrollment.Id, null), CancellationToken.None);

        response.Value!.WasRemoved.ShouldBeTrue();
        builder.Enrollments.Verify(repository => repository.Remove(futureEnrollment), Times.Once);
    }
}
