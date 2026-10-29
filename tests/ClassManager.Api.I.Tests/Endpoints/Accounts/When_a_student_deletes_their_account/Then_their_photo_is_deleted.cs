using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_a_student_deletes_their_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_their_photo_is_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_their_photo_is_deleted_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var photo = await scenario.Student.ChangeMyPhotoAsync();

        await scenario.DeleteAccountAsync();

        using var download = await fixture.AnonymousClient.GetAsync(new Uri(photo.Url!));
        download.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await using var context = fixture.CreateDbContext(scenario.Instructors.Business.Business.Id);
        (await context.UserPhotos.AnyAsync()).ShouldBeFalse();
    }
}
