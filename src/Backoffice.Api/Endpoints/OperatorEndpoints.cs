using ClassManager.Backoffice.Api.Authorization;
using ClassManager.Backoffice.Api.ErrorHandling;
using ClassManager.Backoffice.Core.UseCases;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases;

namespace ClassManager.Backoffice.Api.Endpoints;

internal static class OperatorEndpoints
{
    public static IEndpointRouteBuilder MapOperatorEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(BackofficeApiRoutes.Operators)
            .RequirePlatformPermission(Permissions.Platform.Manage);

        group.MapGet(string.Empty, async (IUseCase<ListOperatorsQuery, OperatorListResponse> useCase, CancellationToken cancellationToken) =>
            (await useCase.ExecuteAsync(new ListOperatorsQuery(), cancellationToken)).ToOkResult());
        group.MapPost(string.Empty, async (
            EnableOperatorRequest request,
            IUseCase<EnableOperatorCommand, OperatorLoginResponse> useCase,
            CancellationToken cancellationToken) =>
            (await useCase.ExecuteAsync(new EnableOperatorCommand(request.AccessRequestId, request.MemberId, request.Role), cancellationToken)).ToOkResult());
        group.MapDelete(BackofficeApiRoutes.OperatorById, async (
            Guid loginId,
            IUseCase<DisableOperatorCommand, OperatorLoginResponse> useCase,
            CancellationToken cancellationToken) =>
            (await useCase.ExecuteAsync(new DisableOperatorCommand(loginId), cancellationToken)).ToOkResult());

        return endpoints;
    }
}

internal sealed record EnableOperatorRequest(Guid AccessRequestId, Guid MemberId, BusinessRole Role);
