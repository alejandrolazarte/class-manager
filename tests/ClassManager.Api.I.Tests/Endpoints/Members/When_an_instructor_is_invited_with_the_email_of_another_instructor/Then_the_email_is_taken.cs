using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Users;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_an_instructor_is_invited_with_the_email_of_another_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_email_is_taken(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_email_is_taken_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var email = MemberRequests.UniqueInviteeEmail();
        var firstInstructor = await owner.CreateInstructorAsync("Ana Gómez");
        var secondInstructor = await owner.CreateInstructorAsync("Martina Gómez");
        await owner.InviteAsync(email, BusinessRole.Instructor, firstInstructor.Id);

        using var response = await owner.PostInvitationAsync(email, BusinessRole.Instructor, secondInstructor.Id);

        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(UserErrorCodes.EmailTaken);
    }
}
