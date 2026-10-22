using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Core.U.Tests.UseCases.Instructors;

internal sealed class InstructorUseCaseBuilder
{
    public Mock<IInstructorRepository> Instructors { get; } = new();
    public Mock<IClassGroupRepository> ClassGroups { get; } = new();
    public const string SignInEmail = "laura@example.com";

    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public Mock<IBusinessMemberRepository> Members { get; } = new();
    public Mock<IIdentityService> Identity { get; } = new();
    public Mock<IUserRepository> Users { get; } = new();
    public Mock<IPeopleLock> PeopleLock { get; } = new();
    public Instructor Instructor { get; } = Instructor.Create(TestData.InstructorFullName, TestData.Now).Value!.WithId();

    public InstructorUseCaseBuilder()
    {
        Instructors.Setup(repository => repository.GetForUpdateAsync(Instructor.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Instructor);
    }

    public void WithLinkedMember()
    {
        var accountId = Guid.CreateVersion7();
        Members.Setup(repository => repository.FindAccountIdByInstructorAsync(Instructor.Id, It.IsAny<CancellationToken>())).ReturnsAsync(accountId);
        Identity
            .Setup(service => service.ListAccountsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserAccount(accountId, SignInEmail, Instructor.User.FullName)]);
    }

    public UpdateInstructorUseCase BuildUpdate() =>
        new(Instructors.Object, Members.Object, Identity.Object, Users.Object, PeopleLock.Object, UnitOfWork.Object);
}
