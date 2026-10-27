namespace ClassManager.Core.U.Tests.UseCases.Students.When_DeleteStudent_of_the_attending_contact;

public sealed class Then_the_contact_person_is_kept
{
    [Fact]
    public async Task Then_the_contact_person_is_kept_Run()
    {
        var builder = new DeleteStudentUseCaseBuilder();
        var contact = builder.AttendingContact();

        await builder.Build().ExecuteAsync(builder.Command(contact.Id), CancellationToken.None);

        builder.Client.User.IsDeleted.ShouldBeFalse();
        contact.IsDeleted.ShouldBeTrue();
    }
}
