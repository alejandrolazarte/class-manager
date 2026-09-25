using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.Domain.Students.When_Student_is_created_with_future_birth_date;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var student = Student.Create(Guid.CreateVersion7(), TestData.StudentFullName, TestData.Today.AddDays(1), null, TestData.Today, TestData.Now);

        student.Error!.FieldName.ShouldBe(nameof(Student.BirthDate));
    }
}
