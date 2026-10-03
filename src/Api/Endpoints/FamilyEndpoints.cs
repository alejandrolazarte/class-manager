using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Families;

namespace ClassManager.Api.Endpoints;

internal static class FamilyEndpoints
{
    public static IEndpointRouteBuilder MapFamilyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var family = endpoints.MapGroup(ApiRoutes.Family).RequireFamily().RequireFeature(Features.FamilyApp);
        family.MapGet("/", GetFamilyHomeAsync);
        family.MapGet(ApiRoutes.FamilyShop, GetFamilyShopAsync);
        family.MapGet(ApiRoutes.FamilyOrders, ListFamilyOrdersAsync);
        family.MapPost(ApiRoutes.FamilyOrders, PlaceFamilyOrderAsync);
        family.MapPut(ApiRoutes.FamilyOrders + ApiRoutes.OrderById + ApiRoutes.Cancellation, CancelFamilyOrderAsync);
        family.MapGet(ApiRoutes.FamilyNews, GetFamilyNewsAsync);
        family.MapPut(ApiRoutes.FamilyNews + ApiRoutes.Seen, MarkFamilyNewsSeenAsync);
        family.MapPut(ApiRoutes.StudentAbsence, NotifyAbsenceAsync);
        family.MapDelete(ApiRoutes.StudentAbsence, WithdrawAbsenceAsync);
        family.MapGet(ApiRoutes.StudentMakeups, GetFamilyMakeupsAsync);
        family.MapGet(ApiRoutes.PushKey, GetFamilyPushKeyAsync);
        family.MapPut(ApiRoutes.PushSubscription, SavePushSubscriptionAsync);
        family.MapDelete(ApiRoutes.PushSubscription, RemovePushSubscriptionAsync);
        family.MapPut(ApiRoutes.StudentMakeup, BookMakeupAsync);
        family.MapDelete(ApiRoutes.StudentMakeup, CancelMakeupAsync);
        family.MapGet(ApiRoutes.StudentPackClasses, GetFamilyPackClassesAsync);
        family.MapPut(ApiRoutes.StudentPackClass, BookPackClassAsync);
        family.MapDelete(ApiRoutes.StudentPackClass, CancelPackClassAsync);

        endpoints.MapPost(ApiRoutes.Clients + ApiRoutes.ClientById + ApiRoutes.AppInvitation, InviteFamilyAsync)
            .RequirePermission(Permissions.Students.Manage)
            .RequireFeature(Features.FamilyApp);

        return endpoints;
    }

    private static async Task<IResult> GetFamilyHomeAsync(
        IUseCase<GetFamilyHomeQuery, FamilyHomeResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetFamilyHomeQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> GetFamilyNewsAsync(
        IUseCase<GetFamilyNewsQuery, FamilyNewsResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetFamilyNewsQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> MarkFamilyNewsSeenAsync(
        IUseCase<MarkFamilyNewsSeenCommand, bool> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new MarkFamilyNewsSeenCommand(), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> NotifyAbsenceAsync(
        Guid studentId,
        Guid classGroupId,
        DateOnly sessionDate,
        IUseCase<NotifyAbsenceCommand, bool> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new NotifyAbsenceCommand(studentId, classGroupId, sessionDate), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> WithdrawAbsenceAsync(
        Guid studentId,
        Guid classGroupId,
        DateOnly sessionDate,
        IUseCase<WithdrawAbsenceCommand, bool> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new WithdrawAbsenceCommand(studentId, classGroupId, sessionDate), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> GetFamilyPushKeyAsync(
        IUseCase<GetFamilyPushKeyQuery, FamilyPushKeyResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetFamilyPushKeyQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> SavePushSubscriptionAsync(
        SavePushSubscriptionCommand command,
        IUseCase<SavePushSubscriptionCommand, bool> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> RemovePushSubscriptionAsync(
        string? endpoint,
        IUseCase<RemovePushSubscriptionCommand, bool> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RemovePushSubscriptionCommand(endpoint), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> GetFamilyMakeupsAsync(
        Guid studentId,
        IUseCase<GetFamilyMakeupsQuery, FamilyMakeupsResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetFamilyMakeupsQuery(studentId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> BookMakeupAsync(
        Guid studentId,
        Guid classGroupId,
        DateOnly sessionDate,
        IUseCase<BookMakeupCommand, bool> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new BookMakeupCommand(studentId, classGroupId, sessionDate), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> CancelMakeupAsync(
        Guid studentId,
        Guid classGroupId,
        DateOnly sessionDate,
        IUseCase<CancelMakeupCommand, bool> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new CancelMakeupCommand(studentId, classGroupId, sessionDate), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> GetFamilyPackClassesAsync(
        Guid studentId,
        IUseCase<GetFamilyPackClassesQuery, FamilyPackClassesResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetFamilyPackClassesQuery(studentId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> BookPackClassAsync(
        Guid studentId,
        Guid classGroupId,
        DateOnly sessionDate,
        IUseCase<BookPackClassCommand, bool> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new BookPackClassCommand(studentId, classGroupId, sessionDate), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> CancelPackClassAsync(
        Guid studentId,
        Guid classGroupId,
        DateOnly sessionDate,
        IUseCase<CancelPackClassCommand, bool> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new CancelPackClassCommand(studentId, classGroupId, sessionDate), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> GetFamilyShopAsync(
        IUseCase<GetFamilyShopQuery, FamilyShopResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetFamilyShopQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> ListFamilyOrdersAsync(
        IUseCase<ListFamilyOrdersQuery, IReadOnlyList<FamilyOrderResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListFamilyOrdersQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> PlaceFamilyOrderAsync(
        PlaceFamilyOrderCommand command,
        IUseCase<PlaceFamilyOrderCommand, FamilyOrderResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(order => TypedResults.Created($"{ApiRoutes.Family}{ApiRoutes.FamilyOrders}/{order.Id}", order));
    }

    private static async Task<IResult> CancelFamilyOrderAsync(
        Guid orderId,
        IUseCase<CancelFamilyOrderCommand, FamilyOrderResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new CancelFamilyOrderCommand(orderId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> InviteFamilyAsync(
        Guid clientId,
        InviteFamilyRequest request,
        IUseCase<InviteFamilyCommand, FamilyInvitationResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request.ToCommand(clientId), cancellationToken);

        return result.ToHttpResult(invitation => TypedResults.Created($"{ApiRoutes.Clients}/{clientId}", invitation));
    }
}
