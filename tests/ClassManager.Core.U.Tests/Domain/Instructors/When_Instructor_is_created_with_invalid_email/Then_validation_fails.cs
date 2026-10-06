using ClassManager.Core.Domain.Instructors;

namespace ClassManager.Core.U.Tests.Domain.Instructors.When_Instructor_is_created_with_invalid_email;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var instructor = Instructor.Create(TestData.InstructorFullName, "not-an-email");

        instructor.Error!.FieldName.ShouldBe(nameof(Instructor.Email));
    }
}
