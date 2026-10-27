using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Enrollments;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.Students;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Students;

internal sealed class DeleteStudentUseCaseBuilder
{
    public Mock<IBusinessRepository> Businesses { get; } = new();
    public Mock<IClientRepository> Clients { get; } = new();
    public Mock<IStudentRepository> Students { get; } = new();
    public Mock<IEnrollmentRepository> Enrollments { get; } = new();
    public Mock<IClientAccountRepository> ClientAccounts { get; } = new();
    public Mock<IClientInvitationRepository> Invitations { get; } = new();
    public Mock<IUserRepository> Users { get; } = new();
    public Mock<IPeopleLock> PeopleLock { get; } = new();
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public FakeTimeProvider TimeProvider { get; } = new(TestData.Now);
    public Client Client { get; } = TestData.Client();
    public Student Student { get; }

    public DeleteStudentUseCaseBuilder()
    {
        Student = ClassManager.Core.Domain.Students.Student.Create(Client, TestData.StudentFullName, new DateOnly(2018, 3, 14), null, TestData.Today, TestData.Now).Value!.WithId();
        Businesses.Setup(repository => repository.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Business());
        Students.Setup(repository => repository.FindForUpdateAsync(Student.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Student);
        Enrollments.Setup(repository => repository.ListOpenByStudentForUpdateAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        ClientAccounts.Setup(repository => repository.ListForUpdateByClientAsync(Client.Id, It.IsAny<CancellationToken>())).ReturnsAsync([]);
        Invitations
            .Setup(repository => repository.ListPendingForUpdateByPersonAsync(It.IsAny<long>(), It.IsAny<long?>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        Users.Setup(repository => repository.GetRolesAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PersonRoles(IsClient: false, IsStudent: true, IsInstructor: false, IsTeamMember: false));
    }

    public Student AttendingContact()
    {
        var contact = ClassManager.Core.Domain.Students.Student.CreateAttendingContact(Client, null, null, TestData.Today, TestData.Now).Value!.WithId();
        Students.Setup(repository => repository.FindForUpdateAsync(contact.Id, It.IsAny<CancellationToken>())).ReturnsAsync(contact);
        Users.Setup(repository => repository.GetRolesAsync(Client.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PersonRoles(IsClient: true, IsStudent: true, IsInstructor: false, IsTeamMember: false));
        return contact;
    }

    public Enrollment WithEnrollmentStartingOn(DateOnly startDate)
    {
        var enrollment = Enrollment.Create(Student.Id, TestIds.Next(), startDate, TestData.Now).WithId();
        Enrollments.Setup(repository => repository.ListOpenByStudentForUpdateAsync(Student.Id, It.IsAny<CancellationToken>())).ReturnsAsync([enrollment]);
        return enrollment;
    }

    public ClientAccount WithStudentAppAccount()
    {
        var account = ClientAccount.Create(Client.Id, TestData.UserOfAccount(Guid.CreateVersion7()), TestData.Now, Student.Id).WithId();
        ClientAccounts.Setup(repository => repository.ListForUpdateByClientAsync(Client.Id, It.IsAny<CancellationToken>())).ReturnsAsync([account]);
        return account;
    }

    public DeleteStudentCommand Command(long? studentId = null) => new(studentId ?? Student.Id);

    public DeleteStudentUseCase Build() =>
        new(
            Businesses.Object,
            Clients.Object,
            Students.Object,
            Enrollments.Object,
            ClientAccounts.Object,
            Invitations.Object,
            Users.Object,
            PeopleLock.Object,
            UnitOfWork.Object,
            TimeProvider,
            new EveryAccessScopes());
}
