using System.Net.Http.Headers;
using System.Text;

using ClassManager.Core.UseCases.ImportExport;
using ClassManager.ImportExport.Xlsx;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class ImportRequests
{
    public const string FileFieldName = "file";
    public const string FileName = "datos.csv";
    public const string CsvMediaType = "text/csv";
    public const string WorkbookFileName = "datos.xlsx";

    public static string ModuleRoute(string module, string action) => $"{ApiRoutes.ImportExport}/{module}{action}";

    public static Task<HttpResponseMessage> PostImportFileAsync(this HttpClient httpClient, string module, string action, string csvText) =>
        httpClient.PostImportFileAsync(module, action, Encoding.UTF8.GetBytes(csvText), FileName, CsvMediaType);

    public static Task<HttpResponseMessage> PostImportFileAsync(
        this HttpClient httpClient,
        string module,
        string action,
        byte[] content,
        string fileName,
        string mediaType)
    {
        var fileContent = new ByteArrayContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
        var form = new MultipartFormDataContent { { fileContent, FileFieldName, fileName } };

        return httpClient.PostAsync(new Uri(ModuleRoute(module, action), UriKind.Relative), form);
    }

    public static async Task<byte[]> DownloadWorkbookAsync(this HttpClient httpClient, string module, string action)
    {
        using var response = await httpClient.GetAsync(new Uri(ModuleRoute(module, action), UriKind.Relative));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    public static IReadOnlyList<IReadOnlyList<string>> WorkbookRows(byte[] workbook)
    {
        var data = new XlsxTabularReader().Read(workbook).Data!;
        return [data.Headers, .. data.Rows.Select(row => row.Cells)];
    }

    public static Task<ImportReport> ImportFileAsync(this HttpClient httpClient, string module, string csvText) =>
        httpClient.ImportFileAsync(module, Encoding.UTF8.GetBytes(csvText), FileName, CsvMediaType);

    public static Task<ImportReport> ImportWorkbookAsync(this HttpClient httpClient, string module, byte[] workbook) =>
        httpClient.ImportFileAsync(module, workbook, WorkbookFileName, XlsxTabularWriter.XlsxContentType);

    private static async Task<ImportReport> ImportFileAsync(this HttpClient httpClient, string module, byte[] content, string fileName, string mediaType)
    {
        using var response = await httpClient.PostImportFileAsync(module, ApiRoutes.ImportAction, content, fileName, mediaType);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ImportReport>(ApiRequests.JsonOptions))!;
    }
}
