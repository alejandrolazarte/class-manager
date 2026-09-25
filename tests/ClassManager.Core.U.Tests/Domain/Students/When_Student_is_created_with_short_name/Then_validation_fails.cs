using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.Domain.Students.When_Student_is_created_with_short_name;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var student = Student.Create(Guid.CreateVersion7(), " T ", null, null, TestData.Today, TestData.Now);

        student.Error!.Kind.ShouldBe(ErrorKind.Validation);
        student.Error.FieldName.ShouldBe(nameof(Student.FullName));
    }
}
