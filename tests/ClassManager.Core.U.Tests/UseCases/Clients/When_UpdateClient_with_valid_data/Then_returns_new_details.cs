using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_UpdateClient_with_valid_data;

public sealed class Then_returns_new_details
{
    [Fact]
    public async Task Then_returns_new_details_Run()
    {
        var builder = new UpdateClientUseCaseBuilder();

        var response = await builder.Build().ExecuteAsync(builder.ValidCommand(), CancellationToken.None);

        response.Value.ShouldBe(new ClientResponse(
            builder.Client.Id,
            UpdateClientUseCaseBuilder.NewFullName,
            "+541155667788",
            UpdateClientUseCaseBuilder.NewEmail,
            UpdateClientUseCaseBuilder.NewNotes,
            builder.Client.CreatedAt));
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
