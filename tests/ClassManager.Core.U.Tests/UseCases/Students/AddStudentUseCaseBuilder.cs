using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.UseCases.Students;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Students;

internal sealed class AddStudentUseCaseBuilder
{
    public Mock<IBusinessRepository> Businesses { get; } = new();
    public Mock<IClientRepository> Clients { get; } = new();
    public Mock<IStudentRepository> Students { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public FakeTimeProvider TimeProvider { get; } = new(TestData.Now);

    public AddStudentUseCaseBuilder()
    {
        Businesses.Setup(repository => repository.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Business());
        Clients.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Client());
    }

    public static AddStudentCommand ValidCommand() =>
        new(Guid.CreateVersion7(), TestData.StudentFullName, new DateOnly(2018, 3, 14), "Afraid of deep water");

    public AddStudentUseCase Build() =>
        new(Businesses.Object, Clients.Object, Students.Object, UnitOfWork.Object, TimeProvider);
}
