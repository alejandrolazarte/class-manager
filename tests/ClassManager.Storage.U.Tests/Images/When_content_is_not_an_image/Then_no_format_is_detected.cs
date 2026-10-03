namespace ClassManager.Storage.U.Tests.Images.When_content_is_not_an_image;

public sealed class Then_no_format_is_detected
{
    [Fact]
    public void Then_no_format_is_detected_Run()
    {
        var format = ImageFormat.Detect(ImageSamples.PlainText);

        format.ShouldBeNull();
    }
}
