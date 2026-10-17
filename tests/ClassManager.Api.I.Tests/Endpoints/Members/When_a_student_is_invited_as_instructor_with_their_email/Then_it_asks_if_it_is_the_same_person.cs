using ClassManager.Api.ErrorHandling;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_a_student_is_invited_as_instructor_with_their_email;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_asks_if_it_is_the_same_person(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_asks_if_it_is_the_same_person_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(email);
        var instructor = await business.HttpClient.CreateInstructorAsync(ApiRequests.ClientFullName);

        using var response = await business.HttpClient.PostInvitationAsync(email, BusinessRole.Instructor, instructor.Id);

        await using var context = fixture.CreateDbContext(business.Business.Id);
        var contactUserId = await context.Clients.Where(client => client.Id == family.Id).Select(client => client.UserId).SingleAsync();
        var problem = await response.ReadProblemAsync();
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        problem.GetProperty(ResultHttpExtensions.ErrorCodeExtension).GetString().ShouldBe(UserErrorCodes.SamePerson);
        problem.GetProperty(UserErrorCodes.UserIdDetail).GetGuid().ShouldBe(contactUserId);
    }
}
