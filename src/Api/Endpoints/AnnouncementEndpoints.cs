using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Announcements;

namespace ClassManager.Api.Endpoints;

internal static class AnnouncementEndpoints
{
    public static IEndpointRouteBuilder MapAnnouncementEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var announcements = endpoints.MapGroup(ApiRoutes.Announcements);
        announcements.MapGet("/", ListAnnouncementsAsync).RequirePermission(Permissions.Announcements.Manage);
        announcements.MapPost("/", CreateAnnouncementAsync).RequirePermission(Permissions.Announcements.Manage);
        announcements.MapDelete(ApiRoutes.AnnouncementById, DeleteAnnouncementAsync).RequirePermission(Permissions.Announcements.Manage);

        return endpoints;
    }

    private static async Task<IResult> ListAnnouncementsAsync(
        IUseCase<ListAnnouncementsQuery, IReadOnlyList<AnnouncementResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListAnnouncementsQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> CreateAnnouncementAsync(
        CreateAnnouncementCommand command,
        IUseCase<CreateAnnouncementCommand, AnnouncementResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(announcement => TypedResults.Created($"{ApiRoutes.Announcements}/{announcement.Id}", announcement));
    }

    private static async Task<IResult> DeleteAnnouncementAsync(
        Guid announcementId,
        IUseCase<DeleteAnnouncementCommand, bool> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new DeleteAnnouncementCommand(announcementId), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }
}
