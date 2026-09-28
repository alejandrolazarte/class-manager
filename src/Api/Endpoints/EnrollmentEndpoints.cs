using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Enrollments;

namespace ClassManager.Api.Endpoints;

internal static class EnrollmentEndpoints
{
    public static IEndpointRouteBuilder MapEnrollmentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var classGroups = endpoints.MapGroup(ApiRoutes.ClassGroups);
        classGroups.MapPost(ApiRoutes.ClassGroupById + ApiRoutes.EnrollmentsSegment, EnrollStudentAsync)
            .RequirePermission(Permissions.Enrollments.Manage);
        classGroups.MapGet(ApiRoutes.ClassGroupById + ApiRoutes.EnrollmentsSegment, ListClassRosterAsync)
            .RequirePermission(Permissions.Enrollments.View);

        endpoints.MapGroup(ApiRoutes.Students).MapGet(ApiRoutes.StudentById + ApiRoutes.EnrollmentsSegment, ListStudentEnrollmentsAsync)
            .RequirePermission(Permissions.Enrollments.View);

        endpoints.MapGroup(ApiRoutes.Enrollments).MapPut(ApiRoutes.EnrollmentById + ApiRoutes.End, EndEnrollmentAsync)
            .RequirePermission(Permissions.Enrollments.Manage);

        return endpoints;
    }

    private static async Task<IResult> EnrollStudentAsync(
        Guid classGroupId,
        EnrollStudentRequest request,
        IUseCase<EnrollStudentCommand, EnrollmentResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request.ToCommand(classGroupId), cancellationToken);

        return result.ToHttpResult(enrollment =>
            TypedResults.Created($"{ApiRoutes.ClassGroups}/{classGroupId}{ApiRoutes.EnrollmentsSegment}", enrollment));
    }

    private static async Task<IResult> ListClassRosterAsync(
        Guid classGroupId,
        IUseCase<ListClassRosterQuery, IReadOnlyList<RosterEntryResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListClassRosterQuery(classGroupId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> ListStudentEnrollmentsAsync(
        Guid studentId,
        IUseCase<ListStudentEnrollmentsQuery, IReadOnlyList<StudentEnrollmentResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListStudentEnrollmentsQuery(studentId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> EndEnrollmentAsync(
        Guid enrollmentId,
        EndEnrollmentRequest request,
        IUseCase<EndEnrollmentCommand, EndEnrollmentResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request.ToCommand(enrollmentId), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }
}
