using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.Students;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Students;

internal sealed class UpdateStudentUseCaseBuilder
{
    public const string NewFullName = "Martina Pérez";
    public const string NewNotes = "Moves to the advanced group";
    public const string NewEmail = "martina@example.com";
    public const string SignInEmail = "martina.app@example.com";
    public static readonly DateOnly NewBirthDate = new(2017, 6, 2);

    public Mock<IBusinessRepository> Businesses { get; } = new();
    public Mock<IClientRepository> Clients { get; } = new();
    public Mock<IStudentRepository> Students { get; } = new();
    public Mock<IClientAccountRepository> ClientAccounts { get; } = new();
    public Mock<IIdentityService> Identity { get; } = new();
    public Mock<IUserRepository> Users { get; } = new();
    public Mock<IPeopleLock> PeopleLock { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public FakeTimeProvider TimeProvider { get; } = new(TestData.Now);
    public Client Client { get; } = TestData.Client();
    public Student Student { get; }

    public UpdateStudentUseCaseBuilder()
    {
        Student = ClassManager.Core.Domain.Students.Student.Create(Client, TestData.StudentFullName, new DateOnly(2018, 3, 14), null, TestData.Today, TestData.Now).Value!.WithId();
        Businesses.Setup(repository => repository.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Business());
        Clients.Setup(repository => repository.GetForUpdateAsync(Client.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Client);
        Students.Setup(repository => repository.FindForUpdateAsync(Student.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Student);
        Students.Setup(repository => repository.ListByClientAsync(Client.Id, It.IsAny<CancellationToken>())).ReturnsAsync([Student]);
        ClientAccounts.Setup(repository => repository.ListByClientAsync(Client.Id, It.IsAny<CancellationToken>())).ReturnsAsync([]);
    }

    public Student AttendingContact()
    {
        var contact = ClassManager.Core.Domain.Students.Student.CreateAttendingContact(Client, null, null, TestData.Today, TestData.Now).Value!.WithId();
        Students.Setup(repository => repository.FindForUpdateAsync(contact.Id, It.IsAny<CancellationToken>())).ReturnsAsync(contact);
        return contact;
    }

    public void WithSibling(string fullName)
    {
        var sibling = ClassManager.Core.Domain.Students.Student.Create(Client, fullName, null, null, TestData.Today, TestData.Now).Value!.WithId();
        Students.Setup(repository => repository.FindByClientAndNameAsync(Client.Id, fullName, It.IsAny<CancellationToken>())).ReturnsAsync(sibling);
    }

    public void WithStudentAppAccount()
    {
        var accountId = Guid.CreateVersion7();
        ClientAccounts.Setup(repository => repository.ListByClientAsync(Client.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([ClientAccount.Create(Client.Id, TestData.UserOfAccount(accountId), TestData.Now, Student.Id).WithId()]);
        Identity
            .Setup(service => service.ListAccountsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserAccount(accountId, SignInEmail, NewFullName)]);
    }

    public UpdateStudentCommand ValidCommand(long? studentId = null) =>
        new(studentId ?? Student.Id, NewFullName, NewBirthDate, NewNotes, NewEmail);

    public UpdateStudentUseCase Build() =>
        new(Businesses.Object, Clients.Object, Students.Object, ClientAccounts.Object, Identity.Object, Users.Object, PeopleLock.Object, UnitOfWork.Object, TimeProvider, new EveryAccessScopes());
}
