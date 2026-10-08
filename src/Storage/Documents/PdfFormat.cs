namespace ClassManager.Storage.Documents;

public static class PdfFormat
{
    public const string ContentType = "application/pdf";
    public const string FileExtension = ".pdf";

    private static readonly byte[] PdfSignature = "%PDF-"u8.ToArray();

    public static bool IsPdf(ReadOnlySpan<byte> content) => content.StartsWith(PdfSignature);
}
