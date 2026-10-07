using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.Domain.Students.When_Student_is_created_without_email;

public sealed class Then_it_has_no_email
{
    [Fact]
    public void Then_it_has_no_email_Run()
    {
        var student = Student.Create(Guid.CreateVersion7(), TestData.StudentFullName, null, null, TestData.Today, TestData.Now, "  ");

        student.Value!.Email.ShouldBeNull();
    }
}
