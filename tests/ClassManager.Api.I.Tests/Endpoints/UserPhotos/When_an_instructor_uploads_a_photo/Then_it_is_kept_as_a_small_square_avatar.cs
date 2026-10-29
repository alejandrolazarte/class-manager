using ClassManager.Core.Domain.Users;

namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_an_instructor_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_kept_as_a_small_square_avatar(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_kept_as_a_small_square_avatar_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        var photo = await scenario.Instructor.ChangeMyPhotoAsync();

        using var avatar = await fixture.DownloadImageAsync(photo.Url!);
        avatar.Width.ShouldBe(UserPhoto.SideInPixels);
        avatar.Height.ShouldBe(UserPhoto.SideInPixels);
        (await fixture.AnonymousClient.GetByteArrayAsync(new Uri(photo.Url!))).Take(4).ShouldBe("RIFF"u8.ToArray());
    }
}
