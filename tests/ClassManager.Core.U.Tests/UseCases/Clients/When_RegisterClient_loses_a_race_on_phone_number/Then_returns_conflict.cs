using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_loses_a_race_on_phone_number;

public sealed class Then_returns_conflict
{
    [Fact]
    public async Task Then_returns_conflict_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        var winnerClient = TestData.Client();
        builder.Clients
            .SetupSequence(repository => repository.FindByPhoneNumberAsync(It.IsAny<PhoneNumber>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null)
            .ReturnsAsync(winnerClient);
        builder.UnitOfWork
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UniqueConstraintViolationException());

        var response = await builder.Build().ExecuteAsync(RegisterClientUseCaseBuilder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(ClientErrorCodes.PhoneNumberTaken);
    }
}
