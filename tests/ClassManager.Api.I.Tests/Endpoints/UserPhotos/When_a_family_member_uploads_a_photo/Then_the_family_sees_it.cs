namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_a_family_member_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_family_sees_it(ApiFixture fixture)
{
    private const string AdultMemberFullName = "Julia Pérez";

    [Fact]
    public async Task Then_the_family_sees_it_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        using var member = await fixture.SignInFamilyMemberAsync(
            scenario.Instructors.Business.HttpClient, scenario.ClientId, AdultMemberFullName, StudentAppRequests.AdultMemberBirthDate);

        await member.App.ChangeMyPhotoAsync();

        var family = await scenario.Student.GetStudentAppFamilyAsync();
        family!.Members.Single(person => person.FullName == AdultMemberFullName).PhotoUrl.ShouldNotBeNull();
    }
}
