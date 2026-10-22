using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Measurements;

namespace ClassManager.Api.Endpoints;

internal static class MeasurementEndpoints
{
    public static IEndpointRouteBuilder MapMeasurementEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var measurements = endpoints.MapGroup(ApiRoutes.Measurements);
        measurements.MapGet("/", ListMeasurementsAsync).RequirePermission(Permissions.Students.ViewOwn);
        measurements.MapPost("/", CreateMeasurementAsync).RequirePermission(Permissions.Business.Manage);
        measurements.MapPut(ApiRoutes.MeasurementById, UpdateMeasurementAsync).RequirePermission(Permissions.Business.Manage);
        measurements.MapDelete(ApiRoutes.MeasurementById, DeleteMeasurementAsync).RequirePermission(Permissions.Business.Manage);
        measurements.MapPost(ApiRoutes.MeasurementById + ApiRoutes.Restore, RestoreMeasurementAsync).RequirePermission(Permissions.Business.Manage);

        var studentMeasurements = endpoints.MapGroup(ApiRoutes.Students + ApiRoutes.StudentById + ApiRoutes.StudentMeasurements);
        studentMeasurements.MapGet("/", GetStudentMeasurementsAsync).RequirePermission(Permissions.Students.ViewOwn);
        studentMeasurements.MapPost(ApiRoutes.MeasurementById + ApiRoutes.Records, RecordMeasurementAsync).RequirePermission(Permissions.Attendance.RecordOwn);
        studentMeasurements.MapPut(ApiRoutes.MeasurementById + ApiRoutes.Goal, SetMeasurementGoalAsync).RequirePermission(Permissions.Attendance.RecordOwn);

        var records = endpoints.MapGroup(ApiRoutes.MeasurementRecords);
        records.MapGet("/", ListClassMeasurementRecordsAsync).RequirePermission(Permissions.Students.ViewOwn);
        records.MapPost("/", RecordClassMeasurementsAsync).RequirePermission(Permissions.Attendance.RecordOwn);
        records.MapDelete(ApiRoutes.MeasurementRecordById, DeleteMeasurementRecordAsync).RequirePermission(Permissions.Attendance.RecordOwn);
        records.MapPost(ApiRoutes.MeasurementRecordById + ApiRoutes.Restore, RestoreMeasurementRecordAsync).RequirePermission(Permissions.Attendance.RecordOwn);

        var studentApp = endpoints.MapGroup(ApiRoutes.StudentApp).RequireStudent().RequireFeature(Features.StudentApp);
        studentApp.MapGet(ApiRoutes.StudentAppMeasurements, GetStudentAppMeasurementsAsync);
        studentApp.MapPost(ApiRoutes.StudentAppMeasurementRecords, RecordStudentAppMeasurementAsync);

        return endpoints;
    }

    private static async Task<IResult> ListMeasurementsAsync(
        IUseCase<ListMeasurementsQuery, IReadOnlyList<MeasurementResponse>> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new ListMeasurementsQuery(), cancellationToken)).ToOkResult();

    private static async Task<IResult> CreateMeasurementAsync(
        CreateMeasurementCommand command,
        IUseCase<CreateMeasurementCommand, MeasurementResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(measurement => TypedResults.Created($"{ApiRoutes.Measurements}/{measurement.Id}", measurement));
    }

    private static async Task<IResult> UpdateMeasurementAsync(
        long measurementId,
        UpdateMeasurementRequest request,
        IUseCase<UpdateMeasurementCommand, MeasurementResponse> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(request.ToCommand(measurementId), cancellationToken)).ToOkResult();

    private static async Task<IResult> DeleteMeasurementAsync(
        long measurementId,
        IUseCase<DeleteMeasurementCommand, bool> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new DeleteMeasurementCommand(measurementId), cancellationToken)).ToHttpResult(_ => TypedResults.NoContent());

    private static async Task<IResult> RestoreMeasurementAsync(
        long measurementId,
        IUseCase<RestoreMeasurementCommand, bool> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new RestoreMeasurementCommand(measurementId), cancellationToken)).ToHttpResult(_ => TypedResults.NoContent());

    private static async Task<IResult> GetStudentMeasurementsAsync(
        long studentId,
        IUseCase<GetStudentMeasurementsQuery, IReadOnlyList<StudentMeasurementResponse>> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new GetStudentMeasurementsQuery(studentId), cancellationToken)).ToOkResult();

    private static async Task<IResult> RecordMeasurementAsync(
        long studentId,
        long measurementId,
        RecordMeasurementRequest request,
        IUseCase<RecordMeasurementCommand, MeasurementRecordResponse> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(request.ToCommand(studentId, measurementId), cancellationToken)).ToOkResult();

    private static async Task<IResult> SetMeasurementGoalAsync(
        long studentId,
        long measurementId,
        SetMeasurementGoalRequest request,
        IUseCase<SetMeasurementGoalCommand, bool> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(request.ToCommand(studentId, measurementId), cancellationToken)).ToHttpResult(_ => TypedResults.NoContent());

    private static async Task<IResult> ListClassMeasurementRecordsAsync(
        DateOnly measuredOn,
        long[] studentIds,
        IUseCase<ListClassMeasurementRecordsQuery, IReadOnlyList<MeasurementRecordResponse>> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new ListClassMeasurementRecordsQuery(measuredOn, studentIds), cancellationToken)).ToOkResult();

    private static async Task<IResult> RecordClassMeasurementsAsync(
        RecordClassMeasurementsCommand command,
        IUseCase<RecordClassMeasurementsCommand, IReadOnlyList<MeasurementRecordResponse>> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(command, cancellationToken)).ToOkResult();

    private static async Task<IResult> DeleteMeasurementRecordAsync(
        long recordId,
        IUseCase<DeleteMeasurementRecordCommand, bool> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new DeleteMeasurementRecordCommand(recordId), cancellationToken)).ToHttpResult(_ => TypedResults.NoContent());

    private static async Task<IResult> RestoreMeasurementRecordAsync(
        long recordId,
        IUseCase<RestoreMeasurementRecordCommand, bool> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new RestoreMeasurementRecordCommand(recordId), cancellationToken)).ToHttpResult(_ => TypedResults.NoContent());

    private static async Task<IResult> GetStudentAppMeasurementsAsync(
        IUseCase<GetStudentAppMeasurementsQuery, StudentAppMeasurementsResponse> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new GetStudentAppMeasurementsQuery(), cancellationToken)).ToOkResult();

    private static async Task<IResult> RecordStudentAppMeasurementAsync(
        long studentId,
        long measurementId,
        RecordMeasurementRequest request,
        IUseCase<RecordStudentAppMeasurementCommand, MeasurementRecordResponse> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(
            new RecordStudentAppMeasurementCommand(studentId, measurementId, request.MeasuredOn, request.Value, request.Note),
            cancellationToken)).ToOkResult();
}
