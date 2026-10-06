using ClassManager.Core.Domain.Instructors;

namespace ClassManager.Core.U.Tests.Domain.Instructors.When_Instructor_is_created_with_blank_email;

public sealed class Then_email_is_empty
{
    [Fact]
    public void Then_email_is_empty_Run()
    {
        var instructor = Instructor.Create(TestData.InstructorFullName, "   ");

        instructor.Value!.Email.ShouldBeNull();
    }
}
