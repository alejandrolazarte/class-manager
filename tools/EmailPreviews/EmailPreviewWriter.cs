using System.Text.Json;
using ClassManager.Notifications.Email;

namespace ClassManager.EmailPreviews;

internal sealed record EmailPreviewFile(string File, string Email, string Brand, string Subject, string TextFile);

internal static class EmailPreviewWriter
{
    public const string ManifestFileName = "manifest.json";

    private const string HtmlExtension = ".html";
    private const string TextExtension = ".txt";
    private const string NameSeparator = "--";
    private const string ContentIdScheme = "cid:";
    private const string DataUriScheme = "data:";
    private const string Base64Marker = ";base64,";

    private static readonly JsonSerializerOptions ManifestOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public static IReadOnlyList<EmailPreviewFile> Write(string outputDirectory, IReadOnlyList<EmailPreview> previews)
    {
        Directory.CreateDirectory(outputDirectory);
        var files = new List<EmailPreviewFile>();
        foreach (var preview in previews)
        {
            var textFileName = preview.Name + TextExtension;
            File.WriteAllText(Path.Combine(outputDirectory, textFileName), preview.Content.ToPlainText());
            var brands = preview.UsesBusinessBrand ? PreviewBrands.Business : [PreviewBrands.App];
            foreach (var brand in brands)
            {
                var fileName = preview.Name + NameSeparator + brand.Name + HtmlExtension;
                File.WriteAllText(Path.Combine(outputDirectory, fileName), RenderForBrowser(preview.Content, brand.Brand));
                files.Add(new EmailPreviewFile(fileName, preview.Name, brand.Name, preview.Subject, textFileName));
            }
        }

        File.WriteAllText(Path.Combine(outputDirectory, ManifestFileName), JsonSerializer.Serialize(files, ManifestOptions));
        return files;
    }

    private static string RenderForBrowser(EmailContent content, EmailBrand brand)
    {
        var html = BrandedEmailHtml.Render(content, brand, EmailPreviewCatalog.WebFontsUrl);
        return brand.Logo is null
            ? html
            : html.Replace(ContentIdScheme + brand.Logo.ContentId, InlineDataUri(brand.Logo), StringComparison.Ordinal);
    }

    private static string InlineDataUri(EmailInlineImage image) =>
        DataUriScheme + image.ContentType + Base64Marker + Convert.ToBase64String(image.Content);
}
