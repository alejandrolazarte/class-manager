using ClassManager.Core.Domain.Users;
using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.Users;

namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_a_client_is_registered_with_the_email_of_an_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_asks_whether_it_is_the_same_person(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_asks_whether_it_is_the_same_person_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var email = MemberRequests.UniqueInviteeEmail();
        var instructor = await owner.CreateInstructorAsync("Ana Gómez", email);

        using var response = await owner.PostClientWithEmailAsync("Ana Gómez", email);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty("code").GetString().ShouldBe(UserErrorCodes.SamePerson);
        SamePersonRequests.SamePersonUserIdOf(problem).ShouldBe(await fixture.UserIdOfInstructorAsync(business.Business.Id, instructor.Id));
        problem.GetProperty(SamePerson.FieldDetail).GetString().ShouldBe(nameof(RegisterClientCommand.Email));
    }
}
