namespace ClassManager.Storage.U.Tests;

internal static class PdfSamples
{
    public static readonly byte[] Pdf = "%PDF-1.4\n1 0 obj\n<<>>\nendobj\ntrailer\n<<>>\n%%EOF\n"u8.ToArray();

    public static readonly byte[] PdfNamedText = "Not a PDF, just text saved as guide.pdf"u8.ToArray();
}
