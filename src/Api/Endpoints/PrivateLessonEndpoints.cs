using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.PrivateLessons;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Api.Endpoints;

internal static class PrivateLessonEndpoints
{
    public static IEndpointRouteBuilder MapPrivateLessonEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var privateLessons = endpoints.MapGroup(ApiRoutes.PrivateLessons);

        privateLessons.MapPost("/", ScheduleAsync).RequireAuthorization(AuthorizationPolicies.OwnerOnly);
        privateLessons.MapGet(ApiRoutes.PrivateLessonById, GetAsync);
        privateLessons.MapPut(ApiRoutes.PrivateLessonById, RescheduleAsync).RequireAuthorization(AuthorizationPolicies.OwnerOnly);
        privateLessons.MapDelete(ApiRoutes.PrivateLessonById, DeleteAsync).RequireAuthorization(AuthorizationPolicies.OwnerOnly);
        privateLessons.MapPut(ApiRoutes.PrivateLessonById + ApiRoutes.Cancellation, CancelAsync)
            .RequireAuthorization(AuthorizationPolicies.OwnerOnly);
        privateLessons.MapDelete(ApiRoutes.PrivateLessonById + ApiRoutes.Cancellation, RestoreAsync)
            .RequireAuthorization(AuthorizationPolicies.OwnerOnly);
        privateLessons.MapPut(ApiRoutes.PrivateLessonById + ApiRoutes.AttendanceByStudent, RecordAttendanceAsync)
            .RequireAuthorization(AuthorizationPolicies.OwnerOnly);

        return endpoints;
    }

    private static async Task<IResult> ScheduleAsync(
        SchedulePrivateLessonCommand command,
        IUseCase<SchedulePrivateLessonCommand, IReadOnlyList<PrivateLessonResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(lessons => TypedResults.Created($"{ApiRoutes.PrivateLessons}/{lessons[0].Id}", lessons));
    }

    private static async Task<IResult> GetAsync(
        Guid privateLessonId,
        IUseCase<GetPrivateLessonQuery, PrivateLessonResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetPrivateLessonQuery(privateLessonId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> RescheduleAsync(
        Guid privateLessonId,
        ReschedulePrivateLessonRequest request,
        IUseCase<ReschedulePrivateLessonCommand, PrivateLessonResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ReschedulePrivateLessonCommand(privateLessonId, request), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> DeleteAsync(
        Guid privateLessonId,
        IUseCase<DeletePrivateLessonCommand, DeletedPrivateLessonResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new DeletePrivateLessonCommand(privateLessonId), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> CancelAsync(
        Guid privateLessonId,
        CancelPrivateLessonRequest request,
        IUseCase<CancelPrivateLessonCommand, PrivateLessonResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new CancelPrivateLessonCommand(privateLessonId, request.Reason), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> RestoreAsync(
        Guid privateLessonId,
        IUseCase<RestorePrivateLessonCommand, PrivateLessonResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RestorePrivateLessonCommand(privateLessonId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> RecordAttendanceAsync(
        Guid privateLessonId,
        Guid studentId,
        RecordAttendanceRequest request,
        IUseCase<RecordPrivateLessonAttendanceCommand, RecordAttendanceResponse> useCase,
        CancellationToken cancellationToken)
    {
        var command = new RecordPrivateLessonAttendanceCommand(privateLessonId, studentId, request.Status);
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }
}
