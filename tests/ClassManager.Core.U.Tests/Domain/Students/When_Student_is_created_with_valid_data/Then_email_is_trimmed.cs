using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.Domain.Students.When_Student_is_created_with_valid_data;

public sealed class Then_email_is_trimmed
{
    [Fact]
    public void Then_email_is_trimmed_Run()
    {
        var student = Student.Create(Guid.CreateVersion7(), TestData.StudentFullName, null, null, TestData.Today, TestData.Now, " tomas@example.com ");

        student.Value!.Email.ShouldBe("tomas@example.com");
    }
}
