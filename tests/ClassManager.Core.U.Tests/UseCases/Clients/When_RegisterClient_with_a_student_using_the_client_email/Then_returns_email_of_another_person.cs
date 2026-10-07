using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_with_a_student_using_the_client_email;

public sealed class Then_returns_email_of_another_person
{
    [Fact]
    public async Task Then_returns_email_of_another_person_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        var command = RegisterClientUseCaseBuilder.ValidCommand() with
        {
            Students = [new NewStudent(TestData.StudentFullName, null, null, " ana@example.com ")],
        };

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        response.Error!.Code.ShouldBe(StudentErrorCodes.EmailOfAnotherPerson);
        response.Error.FieldName.ShouldBe("Students[0].Email");
    }
}
