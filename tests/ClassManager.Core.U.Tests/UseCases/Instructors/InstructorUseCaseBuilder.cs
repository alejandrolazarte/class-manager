using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Instructors;
using ClassManager.Core.Domain.Instructors;

namespace ClassManager.Core.U.Tests.UseCases.Instructors;

internal sealed class InstructorUseCaseBuilder
{
    public Mock<IInstructorRepository> Instructors { get; } = new();
    public Mock<IClassGroupRepository> ClassGroups { get; } = new();
    public const string SignInEmail = "laura@example.com";

    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public Mock<IBusinessMemberRepository> Members { get; } = new();
    public Mock<IIdentityService> Identity { get; } = new();
    public Instructor Instructor { get; } = Instructor.Create(TestData.InstructorFullName).Value!;

    public InstructorUseCaseBuilder()
    {
        Instructors.Setup(repository => repository.GetForUpdateAsync(Instructor.Id, It.IsAny<CancellationToken>())).ReturnsAsync(Instructor);
    }

    public void WithLinkedMember()
    {
        var userId = Guid.CreateVersion7();
        Members.Setup(repository => repository.FindUserIdByInstructorAsync(Instructor.Id, It.IsAny<CancellationToken>())).ReturnsAsync(userId);
        Identity
            .Setup(service => service.ListAccountsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserAccount(userId, SignInEmail, Instructor.FullName)]);
    }

    public UpdateInstructorUseCase BuildUpdate() =>
        new(Instructors.Object, Members.Object, Identity.Object, UnitOfWork.Object);
}
