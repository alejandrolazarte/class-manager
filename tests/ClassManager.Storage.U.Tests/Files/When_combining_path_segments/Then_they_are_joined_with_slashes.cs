namespace ClassManager.Storage.U.Tests.Files.When_combining_path_segments;

public sealed class Then_they_are_joined_with_slashes
{
    [Fact]
    public void Then_they_are_joined_with_slashes_Run()
    {
        var path = FilePath.Combine("tenant", "products", "photo.png");

        path.ShouldBe("tenant/products/photo.png");
    }
}
