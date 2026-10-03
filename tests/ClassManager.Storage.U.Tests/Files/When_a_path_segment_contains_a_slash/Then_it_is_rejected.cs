namespace ClassManager.Storage.U.Tests.Files.When_a_path_segment_contains_a_slash;

public sealed class Then_it_is_rejected
{
    [Fact]
    public void Then_it_is_rejected_Run()
    {
        Should.Throw<ArgumentException>(() => FilePath.Combine("tenant/other", "photo.png"));
    }
}
