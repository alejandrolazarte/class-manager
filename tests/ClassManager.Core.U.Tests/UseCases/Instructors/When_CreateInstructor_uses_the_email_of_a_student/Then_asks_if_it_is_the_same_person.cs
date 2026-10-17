using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.UseCases.Instructors;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Instructors.When_CreateInstructor_uses_the_email_of_a_student;

public sealed class Then_asks_if_it_is_the_same_person
{
    private const string Email = "maria@example.com";

    [Fact]
    public async Task Then_asks_if_it_is_the_same_person_Run()
    {
        var builder = new InstructorUseCaseBuilder();
        var studentUserId = Guid.CreateVersion7();
        builder.Users
            .Setup(repository => repository.FindKnownPersonByEmailAsync(Email, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new KnownPerson(studentUserId, "María Gómez", [PersonRole.Student]));
        var useCase = new CreateInstructorUseCase(builder.Instructors.Object, builder.Users.Object, builder.PeopleLock.Object, builder.UnitOfWork.Object, new FakeTimeProvider(TestData.Now));

        var response = await useCase.ExecuteAsync(new CreateInstructorCommand("María Gómez", Email), CancellationToken.None);

        response.Error!.Code.ShouldBe(UserErrorCodes.SamePerson);
        response.Error.Details[UserErrorCodes.UserIdDetail].ShouldBe(studentUserId);
        builder.UnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
