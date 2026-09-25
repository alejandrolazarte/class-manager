using ClassManager.Core.Domain.Enrollments;

namespace ClassManager.Core.U.Tests.Domain.Enrollments.When_Enrollment_is_already_ended;

public sealed class Then_ending_again_fails
{
    [Fact]
    public void Then_ending_again_fails_Run()
    {
        var enrollment = Enrollment.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), TestData.Today, TestData.Now);
        enrollment.End(TestData.Today.AddDays(10));

        var end = enrollment.End(TestData.Today.AddDays(20));

        end.Error!.Code.ShouldBe(EnrollmentErrorCodes.AlreadyEnded);
    }
}
