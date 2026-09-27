using System.Net.Http.Headers;
using System.Text;

using ClassManager.Core.UseCases.ImportExport;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class ImportRequests
{
    public const string FileFieldName = "file";
    public const string FileName = "datos.csv";
    public const string CsvMediaType = "text/csv";

    public static string ModuleRoute(string module, string action) => $"{ApiRoutes.ImportExport}/{module}{action}";

    public static Task<HttpResponseMessage> PostImportFileAsync(this HttpClient httpClient, string module, string action, string csvText)
    {
        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes(csvText));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(CsvMediaType);
        var form = new MultipartFormDataContent { { fileContent, FileFieldName, FileName } };

        return httpClient.PostAsync(new Uri(ModuleRoute(module, action), UriKind.Relative), form);
    }

    public static async Task<ImportReport> ImportFileAsync(this HttpClient httpClient, string module, string csvText)
    {
        using var response = await httpClient.PostImportFileAsync(module, ApiRoutes.ImportAction, csvText);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ImportReport>(ApiRequests.JsonOptions))!;
    }
}
