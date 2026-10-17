using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_the_instructors_user_has_another_name;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_list_shows_the_users_name(ApiFixture fixture)
{
    private const string UserFullName = "Marta Inés Ruiz";

    [Fact]
    public async Task Then_the_list_shows_the_users_name_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync("Marta Ruiz");
        await fixture.ChangeUserOfInstructorAsync(business.Business.Id, instructor.Id, UserFullName);

        var instructors = await business.HttpClient.GetFromJsonAsync<List<InstructorResponse>>(new Uri(ApiRoutes.Instructors, UriKind.Relative), ApiRequests.JsonOptions);

        instructors!.Single().FullName.ShouldBe(UserFullName);
    }
}
