namespace ClassManager.Core.U.Tests.UseCases.Students.When_UpdateStudent_of_the_attending_contact;

public sealed class Then_the_name_of_the_contact_is_kept
{
    [Fact]
    public async Task Then_the_name_of_the_contact_is_kept_Run()
    {
        var builder = new UpdateStudentUseCaseBuilder();
        var contact = builder.AttendingContact();

        var response = await builder.Build().ExecuteAsync(builder.ValidCommand(contact.Id) with { BirthDate = new DateOnly(1985, 1, 20) }, CancellationToken.None);

        response.Value!.FullName.ShouldBe(TestData.ClientFullName);
        response.Value.Notes.ShouldBe(UpdateStudentUseCaseBuilder.NewNotes);
    }
}
