using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class ApiRequests
{
    public const string ClientFullName = "Ana Pérez";
    public const string ClientPhoneNumber = "11 2233-4455";
    public const string NormalizedClientPhoneNumber = "+541122334455";

    public const string JsonMediaType = "application/json";
    public const string ProblemJsonMediaType = "application/problem+json";
    public const string InternalNamespacePrefix = "ClassManager.";

    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    public static Task<HttpResponseMessage> PostClientAsync(
        this HttpClient httpClient,
        string fullName = ClientFullName,
        string phoneNumber = ClientPhoneNumber) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.Clients,
            new RegisterClientCommand(fullName, phoneNumber, null, null),
            JsonOptions);

    public static Task<HttpResponseMessage> PostRawClientBodyAsync(this HttpClient httpClient, byte[] body)
    {
        var content = new ByteArrayContent(body);
        content.Headers.ContentType = new MediaTypeHeaderValue(JsonMediaType);
        return httpClient.PostAsync(new Uri(ApiRoutes.Clients, UriKind.Relative), content);
    }

    public static async Task<ClientResponse> RegisterClientAsync(
        this HttpClient httpClient,
        string fullName = ClientFullName,
        string phoneNumber = ClientPhoneNumber)
    {
        using var response = await httpClient.PostClientAsync(fullName, phoneNumber);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ClientResponse>(JsonOptions))!;
    }

    public static async Task<JsonElement> ReadProblemAsync(this HttpResponseMessage response) =>
        await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
}
