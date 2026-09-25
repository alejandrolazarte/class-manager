using ClassManager.Core.Domain.Enrollments;

namespace ClassManager.Core.U.Tests.Domain.Enrollments.When_Enrollment_ends_on_a_date;

public sealed class Then_it_is_current_until_that_date
{
    [Fact]
    public void Then_it_is_current_until_that_date_Run()
    {
        var enrollment = Enrollment.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), TestData.Today, TestData.Now);

        enrollment.End(TestData.Today.AddDays(10));

        enrollment.IsCurrentOn(TestData.Today.AddDays(10)).ShouldBeTrue();
        enrollment.IsCurrentOn(TestData.Today.AddDays(11)).ShouldBeFalse();
    }
}
