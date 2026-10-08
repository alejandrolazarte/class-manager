using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.Endpoints;

internal static class StudentAppEndpoints
{
    public static IEndpointRouteBuilder MapStudentAppEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var student = endpoints.MapGroup(ApiRoutes.StudentApp).RequireStudent().RequireFeature(Features.StudentApp);
        student.MapGet("/", GetStudentAppHomeAsync);
        student.MapGet(ApiRoutes.StudentAppShop, GetStudentAppShopAsync);
        student.MapGet(ApiRoutes.StudentAppOrders, ListStudentAppOrdersAsync);
        student.MapPost(ApiRoutes.StudentAppOrders, PlaceStudentAppOrderAsync);
        student.MapPut(ApiRoutes.StudentAppOrders + ApiRoutes.OrderById + ApiRoutes.Cancellation, CancelStudentAppOrderAsync);
        student.MapGet(ApiRoutes.StudentAppNews, GetStudentAppNewsAsync);
        student.MapPost(ApiRoutes.StudentAppGuardianConsent + ApiRoutes.Authorization, GiveGuardianConsentInAppAsync);
        student.MapPost(ApiRoutes.StudentAppGuardianConsent + ApiRoutes.Refusal, RefuseGuardianConsentInAppAsync);
        student.MapPut(ApiRoutes.StudentAppNews + ApiRoutes.Seen, MarkStudentAppNewsSeenAsync);
        student.MapPut(ApiRoutes.StudentAbsence, NotifyAbsenceAsync);
        student.MapDelete(ApiRoutes.StudentAbsence, WithdrawAbsenceAsync);
        student.MapGet(ApiRoutes.StudentMakeups, GetStudentAppMakeupsAsync);
        student.MapGet(ApiRoutes.PushKey, GetStudentAppPushKeyAsync);
        student.MapPut(ApiRoutes.PushSubscription, SavePushSubscriptionAsync);
        student.MapDelete(ApiRoutes.PushSubscription, RemovePushSubscriptionAsync);
        student.MapPut(ApiRoutes.StudentMakeup, BookMakeupAsync);
        student.MapDelete(ApiRoutes.StudentMakeup, CancelMakeupAsync);
        student.MapGet(ApiRoutes.StudentPackClasses, GetStudentAppPackClassesAsync);
        student.MapPut(ApiRoutes.StudentPackClass, BookPackClassAsync);
        student.MapDelete(ApiRoutes.StudentPackClass, CancelPackClassAsync);

        endpoints.MapPost(ApiRoutes.Clients + ApiRoutes.ClientById + ApiRoutes.AppInvitation, InviteStudentAppAsync)
            .RequirePermission(Permissions.Students.Manage)
            .RequireFeature(Features.StudentApp);

        return endpoints;
    }

    private static async Task<IResult> GetStudentAppHomeAsync(
        IUseCase<GetStudentAppHomeQuery, StudentAppHomeResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetStudentAppHomeQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> GiveGuardianConsentInAppAsync(
        Guid invitationId,
        IUseCase<GiveGuardianConsentInAppCommand, GivenGuardianConsentResponse> useCase,
        CancellationToken cancellationToken) =>
        (await useCase.ExecuteAsync(new GiveGuardianConsentInAppCommand(invitationId), cancellationToken)).ToOkResult();

    private static async Task<IResult> RefuseGuardianConsentInAppAsync(
        Guid invitationId,
        IUseCase<RefuseGuardianConsentInAppCommand, RefusedGuardianConsentResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RefuseGuardianConsentInAppCommand(invitationId), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> GetStudentAppNewsAsync(
        IUseCase<GetStudentAppNewsQuery, StudentAppNewsResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetStudentAppNewsQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> MarkStudentAppNewsSeenAsync(
        IUseCase<MarkStudentAppNewsSeenCommand, bool> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new MarkStudentAppNewsSeenCommand(), cancellationToken);

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

    private static async Task<IResult> GetStudentAppPushKeyAsync(
        IUseCase<GetStudentAppPushKeyQuery, StudentAppPushKeyResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetStudentAppPushKeyQuery(), cancellationToken);

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

    private static async Task<IResult> GetStudentAppMakeupsAsync(
        Guid studentId,
        IUseCase<GetStudentAppMakeupsQuery, StudentAppMakeupsResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetStudentAppMakeupsQuery(studentId), cancellationToken);

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

    private static async Task<IResult> GetStudentAppPackClassesAsync(
        Guid studentId,
        IUseCase<GetStudentAppPackClassesQuery, StudentAppPackClassesResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetStudentAppPackClassesQuery(studentId), cancellationToken);

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

    private static async Task<IResult> GetStudentAppShopAsync(
        IUseCase<GetStudentAppShopQuery, StudentAppShopResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetStudentAppShopQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> ListStudentAppOrdersAsync(
        IUseCase<ListStudentAppOrdersQuery, IReadOnlyList<StudentAppOrderResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListStudentAppOrdersQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> PlaceStudentAppOrderAsync(
        PlaceStudentAppOrderCommand command,
        IUseCase<PlaceStudentAppOrderCommand, StudentAppOrderResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(order => TypedResults.Created($"{ApiRoutes.StudentApp}{ApiRoutes.StudentAppOrders}/{order.Id}", order));
    }

    private static async Task<IResult> CancelStudentAppOrderAsync(
        Guid orderId,
        IUseCase<CancelStudentAppOrderCommand, StudentAppOrderResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new CancelStudentAppOrderCommand(orderId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> InviteStudentAppAsync(
        Guid clientId,
        InviteStudentAppRequest request,
        IUseCase<InviteStudentAppCommand, StudentAppInvitationResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request.ToCommand(clientId), cancellationToken);

        return result.ToHttpResult(invitation => TypedResults.Created($"{ApiRoutes.Clients}/{clientId}", invitation));
    }
}
