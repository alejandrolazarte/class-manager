namespace ClassManager.Storage.U.Tests.Images.When_content_is_a_webp;

public sealed class Then_webp_is_detected
{
    [Fact]
    public void Then_webp_is_detected_Run()
    {
        var format = ImageFormat.Detect(ImageSamples.Webp);

        format.ShouldBe(ImageFormat.Webp);
    }
}
