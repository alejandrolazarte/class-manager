namespace ClassManager.Storage.U.Tests.Documents.When_content_is_a_pdf;

public sealed class Then_it_is_recognized
{
    [Fact]
    public void Then_it_is_recognized_Run()
    {
        PdfFormat.IsPdf(PdfSamples.Pdf).ShouldBeTrue();
    }
}
