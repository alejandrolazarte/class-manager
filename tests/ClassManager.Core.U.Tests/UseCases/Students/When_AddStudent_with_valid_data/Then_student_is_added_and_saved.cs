using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.UseCases.Students.When_AddStudent_with_valid_data;

public sealed class Then_student_is_added_and_saved
{
    [Fact]
    public async Task Then_student_is_added_and_saved_Run()
    {
        var builder = new AddStudentUseCaseBuilder();
        var command = AddStudentUseCaseBuilder.ValidCommand();

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        response.Value!.ClientId.ShouldBe(command.ClientId);
        builder.Students.Verify(repository => repository.Add(It.IsAny<Student>()), Times.Once);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
