using ClassManager.Infrastructure.Email;

namespace ClassManager.Api.I.Tests.Email.When_brand_color_is_too_light;

public sealed class Then_the_header_is_darkened_until_white_text_reads
{
    private const double MinimumTextContrastRatio = 4.5;

    [Fact]
    public void Then_the_header_is_darkened_until_white_text_reads_Run()
    {
        var palette = EmailPalette.From("#ffe066", null);

        EmailPalette.ContrastRatio(palette.Primary, EmailPalette.White).ShouldBeGreaterThanOrEqualTo(MinimumTextContrastRatio);
    }
}
