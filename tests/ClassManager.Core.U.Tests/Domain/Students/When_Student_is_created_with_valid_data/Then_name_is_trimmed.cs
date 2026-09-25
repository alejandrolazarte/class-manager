using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.Domain.Students.When_Student_is_created_with_valid_data;

public sealed class Then_name_is_trimmed
{
    [Fact]
    public void Then_name_is_trimmed_Run()
    {
        var clientId = Guid.CreateVersion7();

        var student = Student.Create(clientId, "  Tomás Pérez ", TestData.Today, "  ", TestData.Today, TestData.Now);

        student.Value!.FullName.ShouldBe("Tomás Pérez");
        student.Value.ClientId.ShouldBe(clientId);
        student.Value.Notes.ShouldBeNull();
    }
}
