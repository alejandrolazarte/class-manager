using ClassManager.Core.Domain.Enrollments;

namespace ClassManager.Core.U.Tests.Domain.Enrollments.When_Enrollment_ends_before_it_starts;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var enrollment = Enrollment.Create(TestIds.Next(), TestIds.Next(), TestData.Today, TestData.Now).WithId();

        var end = enrollment.End(TestData.Today.AddDays(-1));

        end.Error!.FieldName.ShouldBe(nameof(Enrollment.EndDate));
    }
}
