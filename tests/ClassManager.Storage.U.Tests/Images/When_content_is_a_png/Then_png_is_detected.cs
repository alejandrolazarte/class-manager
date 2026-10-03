namespace ClassManager.Storage.U.Tests.Images.When_content_is_a_png;

public sealed class Then_png_is_detected
{
    [Fact]
    public void Then_png_is_detected_Run()
    {
        var format = ImageFormat.Detect(ImageSamples.Png);

        format.ShouldBe(ImageFormat.Png);
    }
}
