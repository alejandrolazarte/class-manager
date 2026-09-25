using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_with_too_many_students;

public sealed class Then_nothing_is_saved
{
    [Fact]
    public async Task Then_nothing_is_saved_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        var studentFullNames = Enumerable.Range(1, RegisterClientUseCase.MaximumStudentsPerRegistration + 1).Select(number => $"Student {number}").ToArray();

        var response = await builder.Build().ExecuteAsync(RegisterClientUseCaseBuilder.CommandWithStudents(studentFullNames), CancellationToken.None);

        response.Error!.Kind.ShouldBe(ErrorKind.Validation);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
