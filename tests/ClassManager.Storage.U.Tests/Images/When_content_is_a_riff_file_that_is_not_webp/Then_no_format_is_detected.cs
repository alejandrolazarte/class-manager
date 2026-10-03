namespace ClassManager.Storage.U.Tests.Images.When_content_is_a_riff_file_that_is_not_webp;

public sealed class Then_no_format_is_detected
{
    [Fact]
    public void Then_no_format_is_detected_Run()
    {
        byte[] waveAudio = [.. "RIFF"u8, 0x24, 0x00, 0x00, 0x00, .. "WAVEfmt "u8];

        var format = ImageFormat.Detect(waveAudio);

        format.ShouldBeNull();
    }
}
