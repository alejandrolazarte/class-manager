using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_UpdateClient_with_phone_of_another_client;

public sealed class Then_returns_conflict_with_existing_id
{
    [Fact]
    public async Task Then_returns_conflict_with_existing_id_Run()
    {
        var builder = new UpdateClientUseCaseBuilder();
        var anotherClient = TestData.Client();
        builder.Clients
            .Setup(repository => repository.FindByPhoneNumberAsync(It.IsAny<PhoneNumber>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(anotherClient);

        var response = await builder.Build().ExecuteAsync(builder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(ClientErrorCodes.PhoneNumberTaken);
        response.Error.Details[ClientErrorCodes.ExistingClientIdDetail].ShouldBe(anotherClient.Id);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
