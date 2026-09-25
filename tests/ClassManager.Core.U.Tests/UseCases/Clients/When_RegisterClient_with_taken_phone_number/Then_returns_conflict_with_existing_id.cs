using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_with_taken_phone_number;

public sealed class Then_returns_conflict_with_existing_id
{
    [Fact]
    public async Task Then_returns_conflict_with_existing_id_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        var existingClient = TestData.Client();
        builder.Clients
            .Setup(repository => repository.FindByPhoneNumberAsync(TestData.PhoneNumber(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingClient);

        var response = await builder.Build().ExecuteAsync(RegisterClientUseCaseBuilder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(ClientErrorCodes.PhoneNumberTaken);
        response.Error.Details[ClientErrorCodes.ExistingClientIdDetail].ShouldBe(existingClient.Id);
    }
}
