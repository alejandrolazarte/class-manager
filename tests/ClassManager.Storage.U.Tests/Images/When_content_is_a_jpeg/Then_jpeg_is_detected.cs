namespace ClassManager.Storage.U.Tests.Images.When_content_is_a_jpeg;

public sealed class Then_jpeg_is_detected
{
    [Fact]
    public void Then_jpeg_is_detected_Run()
    {
        var format = ImageFormat.Detect(ImageSamples.Jpeg);

        format.ShouldBe(ImageFormat.Jpeg);
        format!.FileExtension.ShouldBe(".jpg");
    }
}
