using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_with_valid_data;

public sealed class Then_client_is_added_and_saved
{
    [Fact]
    public async Task Then_client_is_added_and_saved_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();

        var response = await builder.Build().ExecuteAsync(RegisterClientUseCaseBuilder.ValidCommand(), CancellationToken.None);

        response.Value!.PhoneNumber.ShouldBe(TestData.NormalizedClientPhoneNumber);
        builder.Clients.Verify(repository => repository.Add(It.IsAny<Client>()), Times.Once);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
