using ClassManager.Core.Domain.Enrollments;

namespace ClassManager.Core.U.Tests.Domain.Enrollments.When_Enrollment_is_already_ended;

public sealed class Then_ending_again_fails
{
    [Fact]
    public void Then_ending_again_fails_Run()
    {
        var enrollment = Enrollment.Create(TestIds.Next(), TestIds.Next(), TestData.Today, TestData.Now).WithId();
        enrollment.End(TestData.Today.AddDays(10));

        var end = enrollment.End(TestData.Today.AddDays(20));

        end.Error!.Code.ShouldBe(EnrollmentErrorCodes.AlreadyEnded);
    }
}
