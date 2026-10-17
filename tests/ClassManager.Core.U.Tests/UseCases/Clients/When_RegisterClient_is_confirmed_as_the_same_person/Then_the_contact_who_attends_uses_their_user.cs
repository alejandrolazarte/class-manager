using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_is_confirmed_as_the_same_person;

public sealed class Then_the_contact_who_attends_uses_their_user
{
    [Fact]
    public async Task Then_the_contact_who_attends_uses_their_user_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        var instructor = User.Create("Ana Pérez", "ana@example.com", TestData.Now).Value!;
        builder.Users
            .Setup(repository => repository.FindKnownPersonByEmailAsync("ana@example.com", It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new KnownPerson(instructor.Id, instructor.FullName, [PersonRole.Instructor]));
        builder.Users.Setup(repository => repository.GetForUpdateAsync(instructor.Id, It.IsAny<CancellationToken>())).ReturnsAsync(instructor);
        Client? addedClient = null;
        Student? addedStudent = null;
        builder.Clients.Setup(repository => repository.Add(It.IsAny<Client>())).Callback<Client>(client => addedClient = client);
        builder.Students.Setup(repository => repository.Add(It.IsAny<Student>())).Callback<Student>(student => addedStudent = student);
        var command = RegisterClientUseCaseBuilder.CommandWithStudents(TestData.ClientFullName) with { SamePersonUserId = instructor.Id };

        await builder.Build().ExecuteAsync(command, CancellationToken.None);

        (addedClient!.User, addedStudent!.User).ShouldBe((instructor, instructor));
    }
}
