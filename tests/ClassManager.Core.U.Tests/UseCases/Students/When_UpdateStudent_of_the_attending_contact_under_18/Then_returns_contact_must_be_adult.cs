using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.UseCases.Students.When_UpdateStudent_of_the_attending_contact_under_18;

public sealed class Then_returns_contact_must_be_adult
{
    [Fact]
    public async Task Then_returns_contact_must_be_adult_Run()
    {
        var builder = new UpdateStudentUseCaseBuilder();
        var contact = builder.AttendingContact();

        var response = await builder.Build().ExecuteAsync(builder.ValidCommand(contact.Id), CancellationToken.None);

        response.Error!.Code.ShouldBe(ClientErrorCodes.ContactMustBeAdult);
    }
}
