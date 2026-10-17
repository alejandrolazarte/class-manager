using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.Instructors;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Instructors.When_CreateInstructor_is_confirmed_as_the_same_person;

public sealed class Then_the_instructor_uses_their_user
{
    private const string Email = "maria@example.com";

    [Fact]
    public async Task Then_the_instructor_uses_their_user_Run()
    {
        var builder = new InstructorUseCaseBuilder();
        var student = User.Create("María Gómez", Email, TestData.Now).Value!;
        builder.Users
            .Setup(repository => repository.FindKnownPersonByEmailAsync(Email, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new KnownPerson(student.Id, student.FullName, [PersonRole.Student]));
        builder.Users.Setup(repository => repository.GetForUpdateAsync(student.Id, It.IsAny<CancellationToken>())).ReturnsAsync(student);
        Instructor? addedInstructor = null;
        builder.Instructors.Setup(repository => repository.Add(It.IsAny<Instructor>())).Callback<Instructor>(instructor => addedInstructor = instructor);
        var useCase = new CreateInstructorUseCase(builder.Instructors.Object, builder.Users.Object, builder.PeopleLock.Object, builder.UnitOfWork.Object, new FakeTimeProvider(TestData.Now));

        await useCase.ExecuteAsync(new CreateInstructorCommand("María Gómez", Email, student.Id), CancellationToken.None);

        addedInstructor!.User.ShouldBeSameAs(student);
    }
}
