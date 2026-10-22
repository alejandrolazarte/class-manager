using ClassManager.Backoffice.Core.Operators;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases;
using ClassManager.Tenancy;

namespace ClassManager.Backoffice.Core.UseCases;

public sealed record ListOperatorsQuery : IQuery;

public sealed record OperatorResponse(
    Guid LoginId,
    string Provider,
    string Email,
    string? FullName,
    BusinessRole? Role,
    string? BranchName,
    DateTimeOffset EnabledAt,
    bool IsCurrentOperator);

public sealed record AccessRequestResponse(Guid Id, string Provider, string Email, string Code, DateTimeOffset RequestedAt);

public sealed record PlatformTeamMemberResponse(Guid MemberId, string FullName, string? Email, string BranchName, BusinessRole Role);

public sealed record OperatorListResponse(
    IReadOnlyList<OperatorResponse> Operators,
    IReadOnlyList<AccessRequestResponse> AccessRequests,
    IReadOnlyList<PlatformTeamMemberResponse> TeamMembers);

public sealed record EnableOperatorCommand(Guid AccessRequestId, Guid MemberId, BusinessRole Role) : ICommand;

public sealed record DisableOperatorCommand(Guid LoginId) : ICommand;

public sealed record OperatorLoginResponse(Guid LoginId);

public sealed class ListOperatorsUseCase(
    IBackofficeLoginRepository loginRepository,
    IPlatformOperatorDirectory operatorDirectory,
    ICurrentUser currentUser)
    : IUseCase<ListOperatorsQuery, OperatorListResponse>
{
    public async Task<Result<OperatorListResponse>> ExecuteAsync(ListOperatorsQuery command, CancellationToken cancellationToken)
    {
        var logins = await loginRepository.ListAsync(cancellationToken);
        var accessRequests = await loginRepository.ListAccessRequestsAsync(cancellationToken);
        var teamMembers = (await operatorDirectory.ListTeamAsync(cancellationToken))
            .Where(member => member.AccountId is not null)
            .ToList();
        var platformMembersByAccount = teamMembers
            .Where(member => SystemRolePermissions.IsPlatformRole(member.Role))
            .GroupBy(member => member.AccountId!.Value)
            .ToDictionary(
                members => members.Key,
                members => members.OrderBy(member => member.Role == BusinessRole.PlatformOperator ? 0 : 1).First());

        return new OperatorListResponse(
            [.. logins
                .OrderBy(login => login.EnabledAt)
                .Select(login => ToOperatorResponse(login, platformMembersByAccount.GetValueOrDefault(login.AccountId)))],
            [.. accessRequests
                .OrderByDescending(request => request.RequestedAt)
                .Select(request => new AccessRequestResponse(request.Id, request.Provider, request.Email, request.Code, request.RequestedAt))],
            [.. teamMembers
                .OrderBy(member => member.FullName, StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(member => member.BranchName, StringComparer.CurrentCultureIgnoreCase)
                .Select(member => new PlatformTeamMemberResponse(member.MemberId, member.FullName, member.Email, member.BranchName, member.Role))]);
    }

    private OperatorResponse ToOperatorResponse(BackofficeLogin login, PlatformTeamMember? member) =>
        new(
            login.Id,
            login.Provider,
            login.Email,
            member?.FullName,
            member?.Role,
            member?.BranchName,
            login.EnabledAt,
            login.AccountId == currentUser.AccountId);
}

public sealed class EnableOperatorUseCase(
    IBackofficeLoginRepository loginRepository,
    IPlatformOperatorDirectory operatorDirectory,
    ITenantScope tenantScope,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    TimeProvider timeProvider)
    : IUseCase<EnableOperatorCommand, OperatorLoginResponse>
{
    private const string MemberIdField = "MemberId";
    private const string RoleField = "Role";

    public async Task<Result<OperatorLoginResponse>> ExecuteAsync(EnableOperatorCommand command, CancellationToken cancellationToken)
    {
        if (await loginRepository.FindAccessRequestByIdAsync(command.AccessRequestId, cancellationToken) is not { } accessRequest)
        {
            return Result.NotFound<OperatorLoginResponse>(OperatorErrorCodes.AccessRequestNotFoundMessage, OperatorErrorCodes.AccessRequestNotFound);
        }

        if (!SystemRolePermissions.IsPlatformRole(command.Role))
        {
            return ValidationError(OperatorErrorCodes.NotAPlatformRoleMessage, OperatorErrorCodes.NotAPlatformRole, RoleField);
        }

        var member = await operatorDirectory.FindTeamMemberAsync(command.MemberId, cancellationToken);
        if (member?.User.AccountId is not { } accountId)
        {
            return ValidationError(OperatorErrorCodes.UnknownTeamMemberMessage, OperatorErrorCodes.UnknownTeamMember, MemberIdField);
        }

        tenantScope.Establish(member.TenantId);
        var roleChange = member.ChangeRole(MemberRole.System(command.Role), instructorId: null);
        if (roleChange.Error is { } roleError)
        {
            return roleError;
        }

        var now = timeProvider.GetUtcNow();
        var identity = accessRequest.ToIdentity();
        var login = await loginRepository.FindAsync(identity.Provider, identity.ProviderKey, cancellationToken);
        if (login is null)
        {
            login = BackofficeLogin.Enable(identity, accountId, now, currentUser.UserId);
            loginRepository.Add(login);
        }
        else
        {
            login.EnableAgain(identity, accountId, now, currentUser.UserId);
        }

        loginRepository.Remove(accessRequest);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await loginRepository.SaveChangesAsync(cancellationToken);
        return new OperatorLoginResponse(login.Id);
    }

    private static Result<OperatorLoginResponse> ValidationError(string message, string code, string fieldName) =>
        new ResultError(code, message, ErrorKind.Validation) { FieldName = fieldName };
}

public sealed class DisableOperatorUseCase(
    IBackofficeLoginRepository loginRepository,
    IBackofficeSessionService sessionService,
    ICurrentUser currentUser)
    : IUseCase<DisableOperatorCommand, OperatorLoginResponse>
{
    public async Task<Result<OperatorLoginResponse>> ExecuteAsync(DisableOperatorCommand command, CancellationToken cancellationToken)
    {
        if (await loginRepository.FindByIdAsync(command.LoginId, cancellationToken) is not { } login)
        {
            return Result.NotFound<OperatorLoginResponse>(OperatorErrorCodes.LoginNotFoundMessage, OperatorErrorCodes.LoginNotFound);
        }

        if (login.AccountId == currentUser.AccountId)
        {
            return Result.Conflict<OperatorLoginResponse>(OperatorErrorCodes.CannotDisableYourselfMessage, OperatorErrorCodes.CannotDisableYourself);
        }

        loginRepository.Remove(login);
        await loginRepository.SaveChangesAsync(cancellationToken);
        await sessionService.RevokeAllAsync(login.AccountId, cancellationToken);
        return new OperatorLoginResponse(login.Id);
    }
}
