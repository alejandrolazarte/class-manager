using ClassManager.Backoffice.Api.Authorization;
using ClassManager.Backoffice.Api.ErrorHandling;
using ClassManager.Backoffice.Core.UseCases;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.UseCases;

namespace ClassManager.Backoffice.Api.Endpoints;

internal static class LogEndpoints
{
    public static IEndpointRouteBuilder MapLogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(BackofficeApiRoutes.CommandLog, async (
            DateOnly? day,
            long? brandId,
            IUseCase<ListCommandLogQuery, CommandLogResponse> useCase,
            CancellationToken cancellationToken) =>
            (await useCase.ExecuteAsync(new ListCommandLogQuery(day, brandId), cancellationToken)).ToOkResult())
            .RequirePlatformPermission(Permissions.Platform.View);
        endpoints.MapGet(BackofficeApiRoutes.SecurityEvents, async (
            DateOnly? day,
            IUseCase<ListSecurityEventsQuery, SecurityEventListResponse> useCase,
            CancellationToken cancellationToken) =>
            (await useCase.ExecuteAsync(new ListSecurityEventsQuery(day), cancellationToken)).ToOkResult())
            .RequirePlatformPermission(Permissions.Platform.View);

        return endpoints;
    }
}
