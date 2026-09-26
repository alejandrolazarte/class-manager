using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Api.Endpoints;

internal static class SessionEndpoints
{
    private const string SessionRoute = ApiRoutes.ClassGroupById + ApiRoutes.SessionsSegment + ApiRoutes.SessionByDate;

    public static IEndpointRouteBuilder MapSessionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup(ApiRoutes.Sessions).MapGet("/", ListDaySessionsAsync);

        var classGroups = endpoints.MapGroup(ApiRoutes.ClassGroups);
        classGroups.MapGet(SessionRoute, GetSessionAsync);
        classGroups.MapPut(SessionRoute + ApiRoutes.AttendanceByStudent, RecordAttendanceAsync)
            .RequireAuthorization(AuthorizationPolicies.OwnerOnly);
        classGroups.MapPut(SessionRoute + ApiRoutes.Cancellation, CancelSessionAsync)
            .RequireAuthorization(AuthorizationPolicies.OwnerOnly);
        classGroups.MapDelete(SessionRoute + ApiRoutes.Cancellation, RestoreSessionAsync)
            .RequireAuthorization(AuthorizationPolicies.OwnerOnly);
        classGroups.MapPut(SessionRoute + ApiRoutes.Schedule, RescheduleSessionAsync)
            .RequireAuthorization(AuthorizationPolicies.OwnerOnly);
        classGroups.MapDelete(SessionRoute + ApiRoutes.Schedule, RestoreSessionScheduleAsync)
            .RequireAuthorization(AuthorizationPolicies.OwnerOnly);

        return endpoints;
    }

    private static async Task<IResult> ListDaySessionsAsync(
        DateOnly? date,
        IUseCase<ListDaySessionsQuery, IReadOnlyList<DaySessionResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListDaySessionsQuery(date), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> GetSessionAsync(
        Guid classGroupId,
        DateOnly sessionDate,
        IUseCase<GetSessionQuery, SessionDetailsResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetSessionQuery(classGroupId, sessionDate), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> RecordAttendanceAsync(
        Guid classGroupId,
        DateOnly sessionDate,
        Guid studentId,
        RecordAttendanceRequest request,
        IUseCase<RecordAttendanceCommand, RecordAttendanceResponse> useCase,
        CancellationToken cancellationToken)
    {
        var command = new RecordAttendanceCommand(classGroupId, sessionDate, studentId, request.Status);
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> CancelSessionAsync(
        Guid classGroupId,
        DateOnly sessionDate,
        CancelSessionRequest request,
        IUseCase<CancelSessionCommand, SessionStatusResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new CancelSessionCommand(classGroupId, sessionDate, request.Reason), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> RestoreSessionAsync(
        Guid classGroupId,
        DateOnly sessionDate,
        IUseCase<RestoreSessionCommand, SessionStatusResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RestoreSessionCommand(classGroupId, sessionDate), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> RescheduleSessionAsync(
        Guid classGroupId,
        DateOnly sessionDate,
        RescheduleSessionRequest request,
        IUseCase<RescheduleSessionCommand, SessionStatusResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RescheduleSessionCommand(classGroupId, sessionDate, request.StartTime), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> RestoreSessionScheduleAsync(
        Guid classGroupId,
        DateOnly sessionDate,
        IUseCase<RestoreSessionScheduleCommand, SessionStatusResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RestoreSessionScheduleCommand(classGroupId, sessionDate), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }
}
