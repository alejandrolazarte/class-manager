using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.UseCases.Students.When_AddStudent_is_confirmed_as_the_same_person;

public sealed class Then_the_student_uses_their_user
{
    private const string Email = "laura@example.com";

    [Fact]
    public async Task Then_the_student_uses_their_user_Run()
    {
        var builder = new AddStudentUseCaseBuilder();
        var instructor = User.Create("Laura Gómez", Email, TestData.Now).Value!;
        builder.Users.HasPersonWithTheirEmail(instructor, new PersonRoles(false, false, true, false));
        Student? addedStudent = null;
        builder.Students.Setup(repository => repository.Add(It.IsAny<Student>())).Callback<Student>(student => addedStudent = student);
        var command = AddStudentUseCaseBuilder.ValidCommand() with { FullName = "Laura Gómez", Email = Email, SamePersonUserId = instructor.Id };

        await builder.Build().ExecuteAsync(command, CancellationToken.None);

        addedStudent!.User.ShouldBeSameAs(instructor);
    }
}
