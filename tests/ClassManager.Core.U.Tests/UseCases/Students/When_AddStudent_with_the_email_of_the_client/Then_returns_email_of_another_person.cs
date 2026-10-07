using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.UseCases.Students.When_AddStudent_with_the_email_of_the_client;

public sealed class Then_returns_email_of_another_person
{
    [Fact]
    public async Task Then_returns_email_of_another_person_Run()
    {
        var builder = new AddStudentUseCaseBuilder();
        var client = Client.Create(TestData.ClientFullName, TestData.PhoneNumber(), "ana@example.com", null, TestData.Now).Value!;
        builder.Clients.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(client);

        var response = await builder.Build().ExecuteAsync(AddStudentUseCaseBuilder.ValidCommand() with { Email = "ANA@example.com" }, CancellationToken.None);

        response.Error!.Code.ShouldBe(StudentErrorCodes.EmailOfAnotherPerson);
        response.Error.FieldName.ShouldBe(nameof(Student.Email));
    }
}
