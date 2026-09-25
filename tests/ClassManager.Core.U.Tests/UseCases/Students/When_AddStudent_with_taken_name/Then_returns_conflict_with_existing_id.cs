using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.UseCases.Students.When_AddStudent_with_taken_name;

public sealed class Then_returns_conflict_with_existing_id
{
    [Fact]
    public async Task Then_returns_conflict_with_existing_id_Run()
    {
        var builder = new AddStudentUseCaseBuilder();
        var existingStudent = Student.Create(Guid.CreateVersion7(), TestData.StudentFullName, null, null, TestData.Today, TestData.Now).Value!;
        builder.Students
            .Setup(repository => repository.FindByClientAndNameAsync(It.IsAny<Guid>(), TestData.StudentFullName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingStudent);

        var response = await builder.Build().ExecuteAsync(AddStudentUseCaseBuilder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(StudentErrorCodes.AlreadyRegistered);
        response.Error.Details[StudentErrorCodes.ExistingStudentIdDetail].ShouldBe(existingStudent.Id);
    }
}
