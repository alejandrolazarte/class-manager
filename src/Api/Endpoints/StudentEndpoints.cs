using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.Endpoints;

internal static class StudentEndpoints
{
    public static IEndpointRouteBuilder MapStudentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup(ApiRoutes.Clients).MapPost(ApiRoutes.ClientById + ApiRoutes.StudentsOfClient, AddStudentAsync)
            .RequirePermission(Permissions.Students.Manage);

        var students = endpoints.MapGroup(ApiRoutes.Students);
        students.MapGet(ApiRoutes.StudentById, GetStudentAsync).RequirePermission(Permissions.Students.ViewAll, Permissions.Students.ViewOwn);
        students.MapGet("/", SearchStudentsAsync).RequirePermission(Permissions.Students.ViewAll, Permissions.Students.ViewOwn);

        return endpoints;
    }

    private static async Task<IResult> AddStudentAsync(
        Guid clientId,
        NewStudent newStudent,
        IUseCase<AddStudentCommand, StudentResponse> useCase,
        CancellationToken cancellationToken)
    {
        var command = new AddStudentCommand(clientId, newStudent.FullName, newStudent.BirthDate, newStudent.Notes, newStudent.Email);
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(student => TypedResults.Created($"{ApiRoutes.Students}/{student.Id}", student));
    }

    private static async Task<IResult> GetStudentAsync(
        Guid studentId,
        IUseCase<GetStudentQuery, StudentSummaryResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetStudentQuery(studentId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> SearchStudentsAsync(
        string? search,
        int? limit,
        IUseCase<SearchStudentsQuery, IReadOnlyList<StudentSummaryResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new SearchStudentsQuery(search, limit), cancellationToken);

        return result.ToOkResult();
    }
}
