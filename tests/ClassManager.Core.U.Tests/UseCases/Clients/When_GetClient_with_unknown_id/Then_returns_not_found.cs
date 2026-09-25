using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_GetClient_with_unknown_id;

public sealed class Then_returns_not_found
{
    [Fact]
    public async Task Then_returns_not_found_Run()
    {
        var clients = new Mock<IClientRepository>();
        var useCase = new GetClientUseCase(clients.Object, new Mock<IStudentRepository>().Object);

        var response = await useCase.ExecuteAsync(new GetClientQuery(Guid.CreateVersion7()), CancellationToken.None);

        response.Error!.Code.ShouldBe(ClientErrorCodes.NotFound);
    }
}
