using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Clients;

internal sealed class UpdateClientUseCaseBuilder
{
    public const string NewFullName = "Ana María Pérez";
    public const string NewPhoneNumber = "11 5566-7788";
    public const string NewEmail = "ana@example.com";
    public const string NewNotes = "Allergic to chlorine";

    public Mock<IBusinessRepository> Businesses { get; } = new();
    public Mock<IClientRepository> Clients { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public Client Client { get; } = TestData.Client();

    public UpdateClientUseCaseBuilder()
    {
        Businesses.Setup(repository => repository.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Business());
        Clients.Setup(repository => repository.GetForUpdateAsync(Client.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Client);
    }

    public UpdateClientCommand ValidCommand() => new(Client.Id, NewFullName, NewPhoneNumber, NewEmail, NewNotes);

    public UpdateClientUseCase Build() =>
        new(Businesses.Object, Clients.Object, UnitOfWork.Object, new EveryAccessScopes());
}
