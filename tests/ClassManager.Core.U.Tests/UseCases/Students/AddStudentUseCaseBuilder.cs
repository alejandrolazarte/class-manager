using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.UseCases.Students;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Students;

internal sealed class AddStudentUseCaseBuilder
{
    public Mock<IBusinessRepository> Businesses { get; } = new();
    public Mock<IClientRepository> Clients { get; } = new();
    public Mock<IStudentRepository> Students { get; } = new();
    public Mock<IUserRepository> Users { get; } = new();
    public Mock<IPeopleLock> PeopleLock { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public FakeTimeProvider TimeProvider { get; } = new(TestData.Now);

    public AddStudentUseCaseBuilder()
    {
        Businesses.Setup(repository => repository.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Business());
        Clients.Setup(repository => repository.GetForUpdateAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((long clientId, CancellationToken _) => TestData.Client().WithId(clientId));
        Students.Setup(repository => repository.ListByClientAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
    }

    public static AddStudentCommand ValidCommand() =>
        new(TestIds.Next(), TestData.StudentFullName, new DateOnly(2018, 3, 14), "Afraid of deep water");

    public AddStudentUseCase Build() =>
        new(Businesses.Object, Clients.Object, Students.Object, Users.Object, PeopleLock.Object, UnitOfWork.Object, TimeProvider, new EveryAccessScopes());
}
