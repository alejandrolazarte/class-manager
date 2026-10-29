namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_the_client_who_pays_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_other_family_members_see_it(ApiFixture fixture)
{
    private const string AdultMemberFullName = "Julia Pérez";

    [Fact]
    public async Task Then_the_other_family_members_see_it_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        using var member = await fixture.SignInFamilyMemberAsync(
            scenario.Instructors.Business.HttpClient, scenario.ClientId, AdultMemberFullName, StudentAppRequests.AdultMemberBirthDate);

        await scenario.Student.ChangeMyPhotoAsync();

        var family = await member.App.GetStudentAppFamilyAsync();
        family!.Members.Single(person => person.PaysForFamily).PhotoUrl.ShouldNotBeNull();
    }
}
