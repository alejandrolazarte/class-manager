namespace ClassManager.Storage.U.Tests.Documents.When_content_is_not_a_pdf;

public sealed class Then_it_is_not_recognized
{
    [Fact]
    public void Then_it_is_not_recognized_Run()
    {
        PdfFormat.IsPdf(PdfSamples.PdfNamedText).ShouldBeFalse();
    }
}
