using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class ApiRequests
{
    public const string ClientFullName = "Ana Pérez";
    public const string ClientPhoneNumber = "11 2233-4455";
    public const string NormalizedClientPhoneNumber = "+541122334455";
    public const string StudentFullName = "Tomás Pérez";

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
        string phoneNumber = ClientPhoneNumber,
        IReadOnlyList<NewStudent>? students = null) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.Clients,
            new RegisterClientCommand(fullName, phoneNumber, null, null, students),
            JsonOptions);

    public static Task<HttpResponseMessage> PutClientAsync(
        this HttpClient httpClient,
        Guid clientId,
        UpdateClientRequest request) =>
        httpClient.PutAsJsonAsync($"{ApiRoutes.Clients}/{clientId}", request, JsonOptions);

    public static Task<HttpResponseMessage> PostStudentAsync(
        this HttpClient httpClient,
        Guid clientId,
        string fullName = StudentFullName) =>
        httpClient.PostAsJsonAsync(
            $"{ApiRoutes.Clients}/{clientId}{ApiRoutes.StudentsOfClient}",
            new NewStudent(fullName, new DateOnly(2018, 3, 14), null),
            JsonOptions);

    public static async Task<StudentResponse> AddStudentAsync(
        this HttpClient httpClient,
        Guid clientId,
        string fullName = StudentFullName)
    {
        using var response = await httpClient.PostStudentAsync(clientId, fullName);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<StudentResponse>(JsonOptions))!;
    }

    public static Task<HttpResponseMessage> PostRawClientBodyAsync(this HttpClient httpClient, byte[] body)
    {
        var content = new ByteArrayContent(body);
        content.Headers.ContentType = new MediaTypeHeaderValue(JsonMediaType);
        return httpClient.PostAsync(new Uri(ApiRoutes.Clients, UriKind.Relative), content);
    }

    public static async Task<ClientDetailsResponse> RegisterClientAsync(
        this HttpClient httpClient,
        string fullName = ClientFullName,
        string phoneNumber = ClientPhoneNumber,
        IReadOnlyList<NewStudent>? students = null)
    {
        using var response = await httpClient.PostClientAsync(fullName, phoneNumber, students);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ClientDetailsResponse>(JsonOptions))!;
    }

    public static async Task<JsonElement> ReadProblemAsync(this HttpResponseMessage response) =>
        await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
}
