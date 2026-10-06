using ClassManager.Core.Domain.Instructors;

namespace ClassManager.Core.U.Tests.Domain.Instructors.When_Instructor_is_updated_with_email;

public sealed class Then_trimmed_email_is_kept
{
    private const string Email = "lucia@example.com";

    [Fact]
    public void Then_trimmed_email_is_kept_Run()
    {
        var instructor = Instructor.Create(TestData.InstructorFullName).Value!;

        instructor.Update(TestData.InstructorFullName, $"  {Email} ");

        instructor.Email.ShouldBe(Email);
    }
}
