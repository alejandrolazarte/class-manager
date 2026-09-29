using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Businesses;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Api.Endpoints;

internal static class FeeEndpoints
{
    public static IEndpointRouteBuilder MapFeeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut(ApiRoutes.Business + ApiRoutes.MonthlyFee, SetDefaultMonthlyFeeAsync)
            .RequirePermission(Permissions.Business.Manage);

        var clients = endpoints.MapGroup(ApiRoutes.Clients);
        clients.MapPut(ApiRoutes.ClientById + ApiRoutes.BillingPlan, SetClientBillingPlanAsync)
            .RequirePermission(Permissions.Payments.Record);
        clients.MapPost(ApiRoutes.ClientById + ApiRoutes.PaymentsSegment, RecordPaymentAsync)
            .RequirePermission(Permissions.Payments.Record);
        clients.MapGet(ApiRoutes.ClientById + ApiRoutes.PaymentsSegment, ListClientPaymentsAsync)
            .RequirePermission(Permissions.Payments.ViewAll, Permissions.Payments.ViewOwn);

        endpoints.MapGroup(ApiRoutes.Payments).MapDelete(ApiRoutes.PaymentById, DeletePaymentAsync)
            .RequirePermission(Permissions.Payments.Record);

        endpoints.MapGroup(ApiRoutes.Fees).MapGet("/", ListMonthlyFeesAsync)
            .RequirePermission(Permissions.Payments.ViewAll, Permissions.Payments.ViewOwn);

        return endpoints;
    }

    private static async Task<IResult> SetDefaultMonthlyFeeAsync(
        SetMonthlyFeeRequest request,
        IUseCase<SetDefaultMonthlyFeeCommand, BusinessResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new SetDefaultMonthlyFeeCommand(request.Amount, request.EffectiveFrom), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> SetClientBillingPlanAsync(
        Guid clientId,
        SetClientBillingPlanRequest request,
        IUseCase<SetClientBillingPlanCommand, ClientBillingResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request.ToCommand(clientId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> RecordPaymentAsync(
        Guid clientId,
        RecordPaymentRequest request,
        IUseCase<RecordPaymentCommand, PaymentResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request.ToCommand(clientId), cancellationToken);

        return result.ToHttpResult(payment =>
            TypedResults.Created($"{ApiRoutes.Clients}/{clientId}{ApiRoutes.PaymentsSegment}", payment));
    }

    private static async Task<IResult> ListClientPaymentsAsync(
        Guid clientId,
        IUseCase<ListClientPaymentsQuery, IReadOnlyList<PaymentResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListClientPaymentsQuery(clientId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> DeletePaymentAsync(
        Guid paymentId,
        IUseCase<DeletePaymentCommand, PaymentResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new DeletePaymentCommand(paymentId), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> ListMonthlyFeesAsync(
        string? month,
        IUseCase<ListMonthlyFeesQuery, MonthlyFeesResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListMonthlyFeesQuery(month), cancellationToken);

        return result.ToOkResult();
    }
}
