namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_an_owner_who_also_studies_fixes_their_name;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_business_where_they_study_has_the_new_name(ApiFixture fixture)
{
    private const string NewFullName = "Ana María Pérez";

    [Fact]
    public async Task Then_the_business_where_they_study_has_the_new_name_Run()
    {
        var ownerAndStudent = await fixture.SeedOwnerWhoIsAlsoStudentAsync();
        using var ownerClient = fixture.CreateClientWithToken(ownerAndStudent.OwnerTokens.AccessToken);
        using (var response = await ownerClient.PutMyProfileAsync(NewFullName, AuthenticationRequests.AdultBirthDate))
        {
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        var client = await ownerAndStudent.Student.Instructors.Business.HttpClient.GetClientAsync(ownerAndStudent.Student.ClientId);

        client.FullName.ShouldBe(NewFullName);
    }
}
