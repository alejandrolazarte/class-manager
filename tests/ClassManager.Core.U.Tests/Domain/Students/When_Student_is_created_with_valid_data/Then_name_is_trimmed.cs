using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.Domain.Students.When_Student_is_created_with_valid_data;

public sealed class Then_name_is_trimmed
{
    [Fact]
    public void Then_name_is_trimmed_Run()
    {
        var client = TestData.Client();

        var student = Student.Create(client, "  Tomás Pérez ", TestData.Today, "  ", TestData.Today, TestData.Now);

        student.Value!.User.FullName.ShouldBe("Tomás Pérez");
        student.Value.ClientId.ShouldBe(client.Id);
        student.Value.Notes.ShouldBeNull();
    }
}
