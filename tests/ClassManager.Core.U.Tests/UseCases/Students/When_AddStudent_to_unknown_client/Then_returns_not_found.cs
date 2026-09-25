using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Students.When_AddStudent_to_unknown_client;

public sealed class Then_returns_not_found
{
    [Fact]
    public async Task Then_returns_not_found_Run()
    {
        var builder = new AddStudentUseCaseBuilder();
        builder.Clients.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Client?)null);

        var response = await builder.Build().ExecuteAsync(AddStudentUseCaseBuilder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(ClientErrorCodes.NotFound);
    }
}
