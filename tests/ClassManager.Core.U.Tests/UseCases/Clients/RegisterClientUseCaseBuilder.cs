using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.UseCases.Clients;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Clients;

internal sealed class RegisterClientUseCaseBuilder
{
    public Mock<IBusinessRepository> Businesses { get; } = new();
    public Mock<IClientRepository> Clients { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();

    public RegisterClientUseCaseBuilder()
    {
        Businesses.Setup(repository => repository.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Business());
    }

    public static RegisterClientCommand ValidCommand() =>
        new(TestData.ClientFullName, TestData.ClientPhoneNumber, "ana@example.com", "Color 6.1 + 20 vol");

    public RegisterClientUseCase Build() =>
        new(Businesses.Object, Clients.Object, UnitOfWork.Object, new FakeTimeProvider(TestData.Now));
}
