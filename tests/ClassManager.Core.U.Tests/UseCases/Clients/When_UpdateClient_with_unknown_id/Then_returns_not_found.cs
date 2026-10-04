using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_UpdateClient_with_unknown_id;

public sealed class Then_returns_not_found
{
    [Fact]
    public async Task Then_returns_not_found_Run()
    {
        var builder = new UpdateClientUseCaseBuilder();

        var response = await builder.Build().ExecuteAsync(builder.ValidCommand() with { ClientId = Guid.CreateVersion7() }, CancellationToken.None);

        response.Error!.Code.ShouldBe(ClientErrorCodes.NotFound);
    }
}
