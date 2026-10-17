using ClassManager.Core.UseCases.Instructors;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Instructors.When_CreateInstructor_uses_the_email_of_another_person;

public sealed class Then_returns_email_taken
{
    private const string Email = "laura@example.com";

    [Fact]
    public async Task Then_returns_email_taken_Run()
    {
        var builder = new InstructorUseCaseBuilder();
        builder.Users
            .Setup(repository => repository.IsEmailTakenAsync(Email, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var useCase = new CreateInstructorUseCase(builder.Instructors.Object, builder.Users.Object, builder.PeopleLock.Object, builder.UnitOfWork.Object, new FakeTimeProvider(TestData.Now));

        var response = await useCase.ExecuteAsync(new CreateInstructorCommand(TestData.InstructorFullName, Email), CancellationToken.None);

        response.Error!.Code.ShouldBe(UserErrorCodes.EmailTaken);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
