using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Api.Endpoints;

internal static class InstructorEndpoints
{
    public static IEndpointRouteBuilder MapInstructorEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var instructors = endpoints.MapGroup(ApiRoutes.Instructors);

        instructors.MapGet("/", ListInstructorsAsync).RequirePermission(Permissions.Instructors.View);
        instructors.MapPost("/", CreateInstructorAsync).RequirePermission(Permissions.Instructors.Manage);
        instructors.MapPut(ApiRoutes.InstructorById, UpdateInstructorAsync).RequirePermission(Permissions.Instructors.Manage);
        instructors.MapPut(ApiRoutes.InstructorById + ApiRoutes.Active, SetInstructorActiveAsync).RequirePermission(Permissions.Instructors.Manage);

        return endpoints;
    }

    private static async Task<IResult> ListInstructorsAsync(
        bool? includeInactive,
        IUseCase<ListInstructorsQuery, IReadOnlyList<InstructorResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListInstructorsQuery(includeInactive ?? false), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> CreateInstructorAsync(
        CreateInstructorCommand command,
        IUseCase<CreateInstructorCommand, InstructorResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(instructor => TypedResults.Created($"{ApiRoutes.Instructors}/{instructor.Id}", instructor));
    }

    private static async Task<IResult> UpdateInstructorAsync(
        Guid instructorId,
        UpdateInstructorRequest request,
        IUseCase<UpdateInstructorCommand, InstructorResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request.ToCommand(instructorId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> SetInstructorActiveAsync(
        Guid instructorId,
        SetActiveRequest request,
        IUseCase<SetInstructorActiveCommand, InstructorResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new SetInstructorActiveCommand(instructorId, request.IsActive), cancellationToken);

        return result.ToOkResult();
    }
}
