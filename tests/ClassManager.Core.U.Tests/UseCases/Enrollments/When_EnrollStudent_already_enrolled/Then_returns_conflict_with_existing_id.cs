using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Enrollments;

namespace ClassManager.Core.U.Tests.UseCases.Enrollments.When_EnrollStudent_already_enrolled;

public sealed class Then_returns_conflict_with_existing_id
{
    [Fact]
    public async Task Then_returns_conflict_with_existing_id_Run()
    {
        var builder = new EnrollmentUseCaseBuilder();
        var existingEnrollment = builder.ExistingEnrollment(TestData.Today);
        builder.Enrollments
            .Setup(repository => repository.FindCurrentAsync(builder.Student.Id, builder.ClassGroup.Id, TestData.Today, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEnrollment);

        var response = await builder.BuildEnroll().ExecuteAsync(builder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(EnrollmentErrorCodes.AlreadyEnrolled);
        response.Error.Details[EnrollmentErrorCodes.ExistingEnrollmentIdDetail].ShouldBe(existingEnrollment.Id);
    }
}
