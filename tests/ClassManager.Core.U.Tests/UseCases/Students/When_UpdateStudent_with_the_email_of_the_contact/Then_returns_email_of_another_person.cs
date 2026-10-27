using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.U.Tests.UseCases.Students.When_UpdateStudent_with_the_email_of_the_contact;

public sealed class Then_returns_email_of_another_person
{
    [Fact]
    public async Task Then_returns_email_of_another_person_Run()
    {
        var builder = new UpdateStudentUseCaseBuilder();
        builder.Client.User.ChangeEmail(UpdateStudentUseCaseBuilder.NewEmail);

        var response = await builder.Build().ExecuteAsync(builder.ValidCommand(), CancellationToken.None);

        response.Error!.Code.ShouldBe(StudentErrorCodes.EmailOfAnotherPerson);
    }
}
