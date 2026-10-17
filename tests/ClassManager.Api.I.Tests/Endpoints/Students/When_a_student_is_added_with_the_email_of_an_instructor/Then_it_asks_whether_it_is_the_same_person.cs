using ClassManager.Core.Domain.Users;

namespace ClassManager.Api.I.Tests.Endpoints.Students.When_a_student_is_added_with_the_email_of_an_instructor;

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
        var client = await owner.RegisterClientAsync();

        using var response = await owner.PostStudentAsync(client.Id, "Ana Gómez", email);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty("code").GetString().ShouldBe(UserErrorCodes.SamePerson);
        SamePersonRequests.SamePersonUserIdOf(problem).ShouldBe(await fixture.UserIdOfInstructorAsync(business.Business.Id, instructor.Id));
    }
}
