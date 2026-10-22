using ClassManager.Core.Domain.Measurements;
using ClassManager.Core.UseCases.Measurements;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class MeasurementRequests
{
    public const string TimeMeasurementName = "25 m crol";
    public const string DistanceMeasurementName = "Nado continuo";

    public static CreateMeasurementCommand TimeMeasurement(bool isVisibleToStudents = true) =>
        new(TimeMeasurementName, MeasurementKind.Time, MeasurementUnits.Seconds, ImprovementDirection.Decreases, isVisibleToStudents, IsRecordedByStudents: false);

    public static CreateMeasurementCommand DistanceRecordedByStudents() =>
        new(DistanceMeasurementName, MeasurementKind.Distance, MeasurementUnits.Meters, ImprovementDirection.Increases, IsVisibleToStudents: true, IsRecordedByStudents: true);

    public static Task<HttpResponseMessage> PostMeasurementAsync(this HttpClient httpClient, CreateMeasurementCommand command) =>
        httpClient.PostAsJsonAsync(ApiRoutes.Measurements, command, ApiRequests.JsonOptions);

    public static async Task<MeasurementResponse> CreateMeasurementAsync(this HttpClient httpClient, CreateMeasurementCommand? command = null)
    {
        using var response = await httpClient.PostMeasurementAsync(command ?? TimeMeasurement());
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<MeasurementResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<List<MeasurementResponse>?> ListMeasurementsAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<List<MeasurementResponse>>(new Uri(ApiRoutes.Measurements, UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> DeleteMeasurementAsync(this HttpClient httpClient, long measurementId) =>
        httpClient.DeleteAsync(new Uri($"{ApiRoutes.Measurements}/{measurementId}", UriKind.Relative));

    public static Task<HttpResponseMessage> RestoreMeasurementAsync(this HttpClient httpClient, long measurementId) =>
        httpClient.PostAsync(new Uri($"{ApiRoutes.Measurements}/{measurementId}{ApiRoutes.Restore}", UriKind.Relative), null);

    public static Task<HttpResponseMessage> PostMeasurementRecordAsync(
        this HttpClient httpClient, long studentId, long measurementId, decimal value, DateOnly? measuredOn = null, string? note = null) =>
        httpClient.PostAsJsonAsync(
            $"{StudentMeasurementsPath(studentId)}/{measurementId}{ApiRoutes.Records}",
            new RecordMeasurementRequest(measuredOn, value, note),
            ApiRequests.JsonOptions);

    public static async Task<MeasurementRecordResponse> RecordMeasurementAsync(
        this HttpClient httpClient, long studentId, long measurementId, decimal value, DateOnly? measuredOn = null)
    {
        using var response = await httpClient.PostMeasurementRecordAsync(studentId, measurementId, value, measuredOn);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<MeasurementRecordResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<List<StudentMeasurementResponse>?> GetStudentMeasurementsAsync(this HttpClient httpClient, long studentId) =>
        httpClient.GetFromJsonAsync<List<StudentMeasurementResponse>>(new Uri(StudentMeasurementsPath(studentId), UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostClassMeasurementsAsync(
        this HttpClient httpClient, long measurementId, IReadOnlyList<ClassMeasurementEntry> entries) =>
        httpClient.PostAsJsonAsync(ApiRoutes.MeasurementRecords, new RecordClassMeasurementsCommand(measurementId, null, entries), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> GetClassMeasurementRecordsAsync(this HttpClient httpClient, DateOnly measuredOn, IReadOnlyList<long> studentIds) =>
        httpClient.GetAsync(new Uri(
            $"{ApiRoutes.MeasurementRecords}?measuredOn={measuredOn:yyyy-MM-dd}{string.Concat(studentIds.Select(studentId => $"&studentIds={studentId}"))}",
            UriKind.Relative));

    public static Task<HttpResponseMessage> DeleteMeasurementRecordAsync(this HttpClient httpClient, long recordId) =>
        httpClient.DeleteAsync(new Uri($"{ApiRoutes.MeasurementRecords}/{recordId}", UriKind.Relative));

    public static Task<HttpResponseMessage> RestoreMeasurementRecordAsync(this HttpClient httpClient, long recordId) =>
        httpClient.PostAsync(new Uri($"{ApiRoutes.MeasurementRecords}/{recordId}{ApiRoutes.Restore}", UriKind.Relative), null);

    public static Task<HttpResponseMessage> PutMeasurementGoalAsync(this HttpClient httpClient, long studentId, long measurementId, decimal? value) =>
        httpClient.PutAsJsonAsync(
            $"{StudentMeasurementsPath(studentId)}/{measurementId}{ApiRoutes.Goal}",
            new SetMeasurementGoalRequest(value),
            ApiRequests.JsonOptions);

    public static Task<StudentAppMeasurementsResponse?> GetStudentAppMeasurementsAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<StudentAppMeasurementsResponse>(
            new Uri(ApiRoutes.StudentApp + ApiRoutes.StudentAppMeasurements, UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostStudentAppMeasurementRecordAsync(this HttpClient httpClient, long studentId, long measurementId, decimal value) =>
        httpClient.PostAsJsonAsync(
            $"{ApiRoutes.StudentApp}/students/{studentId}/measurements/{measurementId}{ApiRoutes.Records}",
            new RecordMeasurementRequest(null, value, null),
            ApiRequests.JsonOptions);

    private static string StudentMeasurementsPath(long studentId) => $"{ApiRoutes.Students}/{studentId}{ApiRoutes.StudentMeasurements}";
}
