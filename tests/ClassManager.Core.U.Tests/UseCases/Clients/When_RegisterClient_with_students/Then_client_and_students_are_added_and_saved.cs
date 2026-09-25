using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_with_students;

public sealed class Then_client_and_students_are_added_and_saved
{
    [Fact]
    public async Task Then_client_and_students_are_added_and_saved_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        var command = RegisterClientUseCaseBuilder.CommandWithStudents("Tomás Pérez", "Lucía Pérez");

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        response.Value!.Students.Select(student => student.FullName).ShouldBe(["Lucía Pérez", "Tomás Pérez"]);
        builder.Students.Verify(repository => repository.Add(It.Is<Student>(student => student.ClientId == response.Value.Id)), Times.Exactly(2));
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
