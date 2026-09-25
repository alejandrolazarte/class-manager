using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.Students;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Clients;

internal sealed class RegisterClientUseCaseBuilder
{
    public Mock<IBusinessRepository> Businesses { get; } = new();
    public Mock<IClientRepository> Clients { get; } = new();
    public Mock<IStudentRepository> Students { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();

    public RegisterClientUseCaseBuilder()
    {
        Businesses.Setup(repository => repository.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Business());
    }

    public static RegisterClientCommand ValidCommand() =>
        new(TestData.ClientFullName, TestData.ClientPhoneNumber, "ana@example.com", "Prefers morning classes", null);

    public static RegisterClientCommand CommandWithStudents(params string[] studentFullNames) =>
        ValidCommand() with { Students = [.. studentFullNames.Select(fullName => new NewStudent(fullName, null, null))] };

    public RegisterClientUseCase Build() =>
        new(Businesses.Object, Clients.Object, Students.Object, UnitOfWork.Object, new FakeTimeProvider(TestData.Now));
}
