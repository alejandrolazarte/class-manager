using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.UserPhotos;

namespace ClassManager.Api.Endpoints;

internal static class UserPhotoEndpoints
{
    public static IEndpointRouteBuilder MapUserPhotoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(ApiRoutes.MyPhoto, GetMyPhotoAsync).RequireAnyAccount();
        endpoints.MapPut(ApiRoutes.MyPhoto, ChangeMyPhotoAsync).RequireAnyAccount().AcceptsPhotoUpload();
        endpoints.MapDelete(ApiRoutes.MyPhoto, RemoveMyPhotoAsync).RequireAnyAccount();

        var approvals = endpoints.MapGroup(ApiRoutes.StudentApp + ApiRoutes.StudentAppPhotoApprovals)
            .RequireStudent()
            .RequireFeature(Features.StudentApp);
        approvals.MapGet("/", ListPhotoApprovalsAsync);
        approvals.MapPost(ApiRoutes.PhotoApprovalById + ApiRoutes.Authorization, ApprovePhotoAsync);
        approvals.MapPost(ApiRoutes.PhotoApprovalById + ApiRoutes.Refusal, RefusePhotoAsync);
        return endpoints;
    }

    private static async Task<IResult> GetMyPhotoAsync(IUseCase<GetMyPhotoQuery, MyPhotoResponse> useCase, CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new GetMyPhotoQuery(), cancellationToken)).ToOkResult();

    private static async Task<IResult> ChangeMyPhotoAsync(
        IFormFile file,
        IUseCase<ChangeMyPhotoCommand, MyPhotoResponse> useCase,
        CancellationToken cancellationToken)
    {
        var content = await ImageUploads.ReadAsync(file, cancellationToken);
        return (await useCase.ExecuteAsync(new ChangeMyPhotoCommand(content), cancellationToken)).ToOkResult();
    }

    private static async Task<IResult> RemoveMyPhotoAsync(IUseCase<RemoveMyPhotoCommand, PhotoDecisionResponse> useCase, CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new RemoveMyPhotoCommand(), cancellationToken)).ToHttpResult(_ => TypedResults.NoContent());

    private static async Task<IResult> ListPhotoApprovalsAsync(
        IUseCase<ListPhotoApprovalsQuery, IReadOnlyList<PhotoApprovalResponse>> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new ListPhotoApprovalsQuery(), cancellationToken)).ToOkResult();

    private static async Task<IResult> ApprovePhotoAsync(
        long photoId,
        IUseCase<ApprovePhotoCommand, PhotoDecisionResponse> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new ApprovePhotoCommand(photoId), cancellationToken)).ToHttpResult(_ => TypedResults.NoContent());

    private static async Task<IResult> RefusePhotoAsync(
        long photoId,
        IUseCase<RefusePhotoCommand, PhotoDecisionResponse> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new RefusePhotoCommand(photoId), cancellationToken)).ToHttpResult(_ => TypedResults.NoContent());
}
