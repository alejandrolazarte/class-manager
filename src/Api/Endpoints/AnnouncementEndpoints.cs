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
        announcements.MapPost(ApiRoutes.AnnouncementById + ApiRoutes.Restore, RestoreAnnouncementAsync).RequirePermission(Permissions.Announcements.Manage);
        announcements.MapPost(ApiRoutes.AnnouncementById + ApiRoutes.Pin, PinAnnouncementAsync).RequirePermission(Permissions.Announcements.Manage);
        announcements.MapDelete(ApiRoutes.AnnouncementById + ApiRoutes.Pin, UnpinAnnouncementAsync).RequirePermission(Permissions.Announcements.Manage);
        announcements.MapGet(ApiRoutes.AnnouncementById + ApiRoutes.Recipients, ListAnnouncementRecipientsAsync)
            .RequirePermission(Permissions.Announcements.Manage);
        announcements.MapPost(ApiRoutes.AnnouncementById + ApiRoutes.Reminder, RemindAnnouncementAsync)
            .RequirePermission(Permissions.Announcements.Manage);
        announcements.MapPost(ApiRoutes.AnnouncementById + ApiRoutes.Images, AddAnnouncementImageAsync)
            .RequirePermission(Permissions.Announcements.Manage)
            .AcceptsImageUpload();
        announcements.MapDelete(ApiRoutes.AnnouncementById + ApiRoutes.ImageById, RemoveAnnouncementImageAsync)
            .RequirePermission(Permissions.Announcements.Manage);

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

    private static async Task<IResult> RestoreAnnouncementAsync(
        Guid announcementId,
        IUseCase<RestoreAnnouncementCommand, bool> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RestoreAnnouncementCommand(announcementId), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> PinAnnouncementAsync(
        Guid announcementId,
        IUseCase<PinAnnouncementCommand, bool> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new PinAnnouncementCommand(announcementId), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> UnpinAnnouncementAsync(
        Guid announcementId,
        IUseCase<UnpinAnnouncementCommand, bool> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new UnpinAnnouncementCommand(announcementId), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> AddAnnouncementImageAsync(
        Guid announcementId,
        IFormFile file,
        IUseCase<AddAnnouncementImageCommand, AnnouncementResponse> useCase,
        CancellationToken cancellationToken)
    {
        var content = await ImageUploads.ReadAsync(file, cancellationToken);
        var result = await useCase.ExecuteAsync(new AddAnnouncementImageCommand(announcementId, content), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> RemoveAnnouncementImageAsync(
        Guid announcementId,
        Guid documentId,
        IUseCase<RemoveAnnouncementImageCommand, AnnouncementResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RemoveAnnouncementImageCommand(announcementId, documentId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> ListAnnouncementRecipientsAsync(
        Guid announcementId,
        IUseCase<ListAnnouncementRecipientsQuery, IReadOnlyList<AnnouncementRecipientResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListAnnouncementRecipientsQuery(announcementId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> RemindAnnouncementAsync(
        Guid announcementId,
        IUseCase<RemindAnnouncementCommand, AnnouncementReminderResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RemindAnnouncementCommand(announcementId), cancellationToken);

        return result.ToOkResult();
    }
}
