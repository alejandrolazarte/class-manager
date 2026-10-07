using System.Security.Claims;
using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Accounts;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.Branches;
using ClassManager.Core.UseCases.Members;
using ClassManager.Core.UseCases.StudentApp;
using ClassManager.Security.Hosting;

namespace ClassManager.Api.Endpoints;

internal static class AuthenticationEndpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var authentication = endpoints.MapGroup(ApiRoutes.Authentication)
            .AllowAnonymous()
            .RequireRateLimiting(SecurityServiceCollectionExtensions.AuthenticationRateLimitPolicy);

        authentication.MapPost(ApiRoutes.SignUp, SignUpAsync);
        authentication.MapPost(ApiRoutes.SignIn, SignInAsync);
        authentication.MapPost(ApiRoutes.Refresh, RefreshAsync);
        authentication.MapPost(ApiRoutes.SignOut, SignOutAsync);
        authentication.MapPost(ApiRoutes.PasswordResetRequest, RequestPasswordResetAsync);
        authentication.MapPost(ApiRoutes.PasswordReset, ResetPasswordAsync);
        authentication.MapPost(ApiRoutes.ConfirmEmailChange, ConfirmEmailChangeAsync);
        authentication.MapPost(ApiRoutes.AcceptInvitation, AcceptInvitationAsync);
        authentication.MapPost(ApiRoutes.AcceptStudentAppInvitation, AcceptStudentAppInvitationAsync);
        authentication.MapPost(ApiRoutes.CheckInvitation, CheckInvitationAsync);
        authentication.MapPost(ApiRoutes.CheckStudentAppInvitation, CheckStudentAppInvitationAsync);
        authentication.MapPost(ApiRoutes.DeclineInvitation, DeclineInvitationAsync);
        authentication.MapPost(ApiRoutes.DeclineStudentAppInvitation, DeclineStudentAppInvitationAsync);
        authentication.MapPost(ApiRoutes.SwitchBranch, SwitchBranchAsync);
        endpoints.MapGet(ApiRoutes.MyAccounts, ListAccountsAsync).RequireAnyAccount();
        endpoints.MapGet(ApiRoutes.MyAccount, GetMyAccountAsync).RequireAnyAccount();
        endpoints.MapPut(ApiRoutes.MyAccount, UpdateMyProfileAsync).RequireAnyAccount();
        endpoints.MapPost(ApiRoutes.MyEmailChange, RequestEmailChangeAsync)
            .RequireAnyAccount()
            .RequireRateLimiting(SecurityServiceCollectionExtensions.AuthenticationRateLimitPolicy);

        return endpoints;
    }

    private static async Task<IResult> SignUpAsync(
        SignUpOwnerCommand command,
        IUseCase<SignUpOwnerCommand, TokenResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(tokens => TypedResults.Created(ApiRoutes.Business, tokens));
    }

    private static async Task<IResult> SignInAsync(
        SignInCommand command,
        IUseCase<SignInCommand, TokenResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> RefreshAsync(
        RefreshSessionCommand command,
        IUseCase<RefreshSessionCommand, TokenResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> SignOutAsync(
        SignOutCommand command,
        IUseCase<SignOutCommand, SignOutResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> RequestPasswordResetAsync(
        RequestPasswordResetCommand command,
        IUseCase<RequestPasswordResetCommand, RequestPasswordResetResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(_ => TypedResults.Accepted((string?)null));
    }

    private static async Task<IResult> ResetPasswordAsync(
        ResetPasswordCommand command,
        IUseCase<ResetPasswordCommand, ResetPasswordResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> GetMyAccountAsync(
        IUseCase<GetMyAccountQuery, MyAccountResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new GetMyAccountQuery(), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> UpdateMyProfileAsync(
        UpdateMyProfileCommand command,
        IUseCase<UpdateMyProfileCommand, MyAccountResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> RequestEmailChangeAsync(
        RequestEmailChangeCommand command,
        IUseCase<RequestEmailChangeCommand, RequestEmailChangeResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(response => TypedResults.Accepted((string?)null, response));
    }

    private static async Task<IResult> ConfirmEmailChangeAsync(
        ConfirmEmailChangeCommand command,
        IUseCase<ConfirmEmailChangeCommand, ConfirmEmailChangeResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> ListAccountsAsync(
        ClaimsPrincipal user,
        IUseCase<ListAccountsQuery, IReadOnlyList<AccountResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListAccountsQuery(SessionKindClaim.Of(user)), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> SwitchBranchAsync(
        SwitchBranchCommand command,
        IUseCase<SwitchBranchCommand, TokenResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> AcceptStudentAppInvitationAsync(
        AcceptStudentAppInvitationCommand command,
        IUseCase<AcceptStudentAppInvitationCommand, TokenResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> AcceptInvitationAsync(
        AcceptInvitationCommand command,
        IUseCase<AcceptInvitationCommand, TokenResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> CheckInvitationAsync(
        CheckInvitationCommand command,
        IUseCase<CheckInvitationCommand, CheckInvitationResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> CheckStudentAppInvitationAsync(
        CheckStudentAppInvitationCommand command,
        IUseCase<CheckStudentAppInvitationCommand, CheckStudentAppInvitationResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> DeclineInvitationAsync(
        DeclineInvitationCommand command,
        IUseCase<DeclineInvitationCommand, DeclinedInvitationResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }

    private static async Task<IResult> DeclineStudentAppInvitationAsync(
        DeclineStudentAppInvitationCommand command,
        IUseCase<DeclineStudentAppInvitationCommand, DeclinedStudentAppInvitationResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(_ => TypedResults.NoContent());
    }
}
