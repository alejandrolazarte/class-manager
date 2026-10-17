using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.UseCases.Instructors;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Instructors.When_CreateInstructor_uses_the_email_of_another_instructor;

public sealed class Then_returns_email_taken
{
    private const string Email = "laura@example.com";

    [Fact]
    public async Task Then_returns_email_taken_Run()
    {
        var builder = new InstructorUseCaseBuilder();
        var otherInstructor = User.Create("Laura Gómez", Email, TestData.Now).Value!;
        builder.Users
            .Setup(repository => repository.FindByEmailForUpdateAsync(Email, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(otherInstructor);
        builder.Users
            .Setup(repository => repository.GetRolesAsync(otherInstructor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PersonRoles(IsClient: false, IsStudent: false, IsInstructor: true, IsTeamMember: false));
        var useCase = new CreateInstructorUseCase(builder.Instructors.Object, builder.Users.Object, builder.PeopleLock.Object, builder.UnitOfWork.Object, new FakeTimeProvider(TestData.Now));

        var response = await useCase.ExecuteAsync(new CreateInstructorCommand(TestData.InstructorFullName, Email), CancellationToken.None);

        response.Error!.Code.ShouldBe(UserErrorCodes.EmailTaken);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
