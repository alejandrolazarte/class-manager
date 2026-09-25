using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.Domain.Students.When_Student_is_created_with_long_notes;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var notes = new string('x', Student.NotesMaxLength + 1);

        var student = Student.Create(Guid.CreateVersion7(), TestData.StudentFullName, null, notes, TestData.Today, TestData.Now);

        student.Error!.FieldName.ShouldBe(nameof(Student.Notes));
    }
}
