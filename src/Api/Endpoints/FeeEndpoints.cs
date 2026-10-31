using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Api.Endpoints;

internal static class FeeEndpoints
{
    public static IEndpointRouteBuilder MapFeeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var students = endpoints.MapGroup(ApiRoutes.Students);
        students.MapPut(ApiRoutes.StudentById + ApiRoutes.StudentPlan, SetStudentPlanAsync)
            .RequirePermission(Permissions.Payments.Record);
        students.MapDelete(ApiRoutes.StudentById + ApiRoutes.StudentPlanChangeByMonth, DeleteStudentPlanChangeAsync)
            .RequirePermission(Permissions.Payments.Record);

        var clients = endpoints.MapGroup(ApiRoutes.Clients);
        clients.MapPost(ApiRoutes.ClientById + ApiRoutes.PaymentsSegment, RecordPaymentAsync)
            .RequirePermission(Permissions.Payments.Record);
        clients.MapGet(ApiRoutes.ClientById + ApiRoutes.PaymentsSegment, ListClientPaymentsAsync)
            .RequirePermission(Permissions.Payments.ViewAll, Permissions.Payments.ViewOwn);

        var payments = endpoints.MapGroup(ApiRoutes.Payments);
        payments.MapDelete(ApiRoutes.PaymentById, DeletePaymentAsync)
            .RequirePermission(Permissions.Payments.Record);
        payments.MapPost(ApiRoutes.PaymentById + ApiRoutes.Restore, RestorePaymentAsync)
            .RequirePermission(Permissions.Payments.Record);

        endpoints.MapGroup(ApiRoutes.Fees).MapGet("/", ListMonthlyFeesAsync)
            .RequirePermission(Permissions.Payments.ViewAll, Permissions.Payments.ViewOwn);

        return endpoints;
    }

    private static async Task<IResult> SetStudentPlanAsync(
        long studentId,
        SetStudentPlanRequest request,
        IUseCase<SetStudentPlanCommand, FamilyPlansResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request.ToCommand(studentId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> DeleteStudentPlanChangeAsync(
        long studentId,
        string month,
        IUseCase<DeleteStudentPlanChangeCommand, FamilyPlansResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new DeleteStudentPlanChangeCommand(studentId, month), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> RecordPaymentAsync(
        long clientId,
        RecordPaymentRequest request,
        IUseCase<RecordPaymentCommand, PaymentResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request.ToCommand(clientId), cancellationToken);

        return result.ToHttpResult(payment =>
            TypedResults.Created($"{ApiRoutes.Clients}/{clientId}{ApiRoutes.PaymentsSegment}", payment));
    }

    private static async Task<IResult> ListClientPaymentsAsync(
        long clientId,
        IUseCase<ListClientPaymentsQuery, IReadOnlyList<PaymentResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListClientPaymentsQuery(clientId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> DeletePaymentAsync(
        long paymentId,
        IUseCase<DeletePaymentCommand, PaymentResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new DeletePaymentCommand(paymentId), cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> RestorePaymentAsync(
        long paymentId,
        IUseCase<RestorePaymentCommand, PaymentResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RestorePaymentCommand(paymentId), cancellationToken);

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
