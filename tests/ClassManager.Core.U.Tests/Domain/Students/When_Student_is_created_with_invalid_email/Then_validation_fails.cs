using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.Domain.Students.When_Student_is_created_with_invalid_email;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var student = Student.Create(Guid.CreateVersion7(), TestData.StudentFullName, null, null, TestData.Today, TestData.Now, "not-an-email");

        student.Error!.FieldName.ShouldBe(nameof(Student.Email));
    }
}
