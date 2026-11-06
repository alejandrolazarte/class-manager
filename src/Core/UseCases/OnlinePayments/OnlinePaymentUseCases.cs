using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.OnlinePayments;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.OnlinePayments;

namespace ClassManager.Core.UseCases.OnlinePayments;

public sealed record GetOnlinePaymentsQuery : IQuery;

public sealed record StartOnlinePaymentOnboardingCommand : ICommand;

public sealed record RefreshOnlinePaymentAccountCommand : ICommand;

public sealed record UseOnlinePaymentAccountCommand(long AccountId) : ICommand;

public sealed record StopUsingOnlinePaymentAccountCommand : ICommand;

public sealed record SyncOnlinePaymentAccountCommand([property: AuditPlain] string ExternalAccountId) : ICommand;

public sealed class GetOnlinePaymentsUseCase(
    IBusinessRepository businessRepository,
    IOnlinePaymentAccountRepository accountRepository,
    ICurrentMember currentMember)
    : IUseCase<GetOnlinePaymentsQuery, OnlinePaymentsResponse>
{
    public async Task<Result<OnlinePaymentsResponse>> ExecuteAsync(GetOnlinePaymentsQuery command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return OnlinePaymentResults.NoBusiness<OnlinePaymentsResponse>();
        }

        return await OnlinePaymentResults.ResponseAsync(business, accountRepository, currentMember, cancellationToken);
    }
}

public sealed class StartOnlinePaymentOnboardingUseCase(
    IBusinessRepository businessRepository,
    IOnlinePaymentAccountRepository accountRepository,
    IOnlinePaymentGateway gateway,
    IIdentityService identityService,
    ICurrentUser currentUser,
    IWebAppLinks webAppLinks,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<StartOnlinePaymentOnboardingCommand, OnlinePaymentRedirectResponse>
{
    public async Task<Result<OnlinePaymentRedirectResponse>> ExecuteAsync(StartOnlinePaymentOnboardingCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentForUpdateAsync(cancellationToken);
        if (business is null)
        {
            return OnlinePaymentResults.NoBusiness<OnlinePaymentRedirectResponse>();
        }

        if (!OnlinePaymentCountries.IsAvailableIn(business.CountryCode))
        {
            return Result.Forbidden<OnlinePaymentRedirectResponse>(OnlinePaymentErrorCodes.NotAvailableMessage, OnlinePaymentErrorCodes.NotAvailable);
        }

        var account = await FindAccountAsync(business, cancellationToken) ?? await CreateAccountAsync(business, cancellationToken);
        business.UseOnlinePaymentAccount(account.Id);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var onboardingUrl = await gateway.CreateOnboardingLinkAsync(
            new OnboardingLinkRequest(
                account.ExternalAccountId,
                new Uri(webAppLinks.OnlinePaymentsOnboardingExpired()),
                new Uri(webAppLinks.OnlinePaymentsOnboardingReturned())),
            cancellationToken);
        return new OnlinePaymentRedirectResponse(onboardingUrl);
    }

    private async Task<OnlinePaymentAccount?> FindAccountAsync(Business business, CancellationToken cancellationToken) =>
        business.OnlinePaymentAccountId is { } accountId
            ? await accountRepository.FindAsync(business.OrganizationId, accountId, cancellationToken)
            : await accountRepository.FindCreatedForAsync(business.Id, cancellationToken);

    private async Task<OnlinePaymentAccount> CreateAccountAsync(Business business, CancellationToken cancellationToken)
    {
        var email = currentUser.AccountId is { } accountId
            && await identityService.ListAccountsAsync([accountId], cancellationToken) is [var userAccount, ..]
            ? userAccount.Email
            : null;
        var externalAccountId = await gateway.CreateAccountAsync(
            new OnlinePaymentAccountRequest(business.OrganizationId, business.Id, business.CountryCode, email),
            cancellationToken);
        var account = OnlinePaymentAccount.Create(business.OrganizationId, business.Id, externalAccountId, timeProvider.GetUtcNow());
        accountRepository.Add(account);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return account;
    }
}

public sealed class RefreshOnlinePaymentAccountUseCase(
    IBusinessRepository businessRepository,
    IOnlinePaymentAccountRepository accountRepository,
    IOnlinePaymentGateway gateway,
    ICurrentMember currentMember,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<RefreshOnlinePaymentAccountCommand, OnlinePaymentsResponse>
{
    public async Task<Result<OnlinePaymentsResponse>> ExecuteAsync(RefreshOnlinePaymentAccountCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (business is null)
        {
            return OnlinePaymentResults.NoBusiness<OnlinePaymentsResponse>();
        }

        if (business.OnlinePaymentAccountId is not { } accountId
            || await accountRepository.FindAsync(business.OrganizationId, accountId, cancellationToken) is not { } account)
        {
            return Result.Conflict<OnlinePaymentsResponse>(OnlinePaymentErrorCodes.NotConnectedMessage, OnlinePaymentErrorCodes.NotConnected);
        }

        if (await gateway.GetAccountAsync(account.ExternalAccountId, cancellationToken) is { } state)
        {
            account.Update(state, timeProvider.GetUtcNow());
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return await OnlinePaymentResults.ResponseAsync(business, accountRepository, currentMember, cancellationToken);
    }
}

public sealed class UseOnlinePaymentAccountUseCase(
    IBusinessRepository businessRepository,
    IOnlinePaymentAccountRepository accountRepository,
    ICurrentMember currentMember,
    IUnitOfWork unitOfWork)
    : IUseCase<UseOnlinePaymentAccountCommand, OnlinePaymentsResponse>
{
    public async Task<Result<OnlinePaymentsResponse>> ExecuteAsync(UseOnlinePaymentAccountCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentForUpdateAsync(cancellationToken);
        if (business is null)
        {
            return OnlinePaymentResults.NoBusiness<OnlinePaymentsResponse>();
        }

        if (await accountRepository.FindAsync(business.OrganizationId, command.AccountId, cancellationToken) is not { } account)
        {
            return Result.NotFound<OnlinePaymentsResponse>(OnlinePaymentErrorCodes.AccountNotFoundMessage, OnlinePaymentErrorCodes.AccountNotFound);
        }

        business.UseOnlinePaymentAccount(account.Id);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await OnlinePaymentResults.ResponseAsync(business, accountRepository, currentMember, cancellationToken);
    }
}

public sealed class StopUsingOnlinePaymentAccountUseCase(
    IBusinessRepository businessRepository,
    IOnlinePaymentAccountRepository accountRepository,
    ICurrentMember currentMember,
    IUnitOfWork unitOfWork)
    : IUseCase<StopUsingOnlinePaymentAccountCommand, OnlinePaymentsResponse>
{
    public async Task<Result<OnlinePaymentsResponse>> ExecuteAsync(StopUsingOnlinePaymentAccountCommand command, CancellationToken cancellationToken)
    {
        var business = await businessRepository.GetCurrentForUpdateAsync(cancellationToken);
        if (business is null)
        {
            return OnlinePaymentResults.NoBusiness<OnlinePaymentsResponse>();
        }

        if (business.OnlinePaymentAccountId is null)
        {
            return Result.Conflict<OnlinePaymentsResponse>(OnlinePaymentErrorCodes.NotConnectedMessage, OnlinePaymentErrorCodes.NotConnected);
        }

        business.StopUsingOnlinePaymentAccount();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await OnlinePaymentResults.ResponseAsync(business, accountRepository, currentMember, cancellationToken);
    }
}

public sealed class SyncOnlinePaymentAccountUseCase(
    IOnlinePaymentAccountRepository accountRepository,
    IOnlinePaymentGateway gateway,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<SyncOnlinePaymentAccountCommand, bool>
{
    public async Task<Result<bool>> ExecuteAsync(SyncOnlinePaymentAccountCommand command, CancellationToken cancellationToken)
    {
        if (await accountRepository.FindByExternalIdAsync(command.ExternalAccountId, cancellationToken) is not { } account
            || await gateway.GetAccountAsync(account.ExternalAccountId, cancellationToken) is not { } state)
        {
            return false;
        }

        account.Update(state, timeProvider.GetUtcNow());
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

internal static class OnlinePaymentResults
{
    public static Result<T> NoBusiness<T>() =>
        Result.Unauthorized<T>(BusinessErrorCodes.CurrentBusinessNotFoundMessage, BusinessErrorCodes.CurrentBusinessNotFound);

    public static async Task<Result<OnlinePaymentsResponse>> ResponseAsync(
        Business business,
        IOnlinePaymentAccountRepository accountRepository,
        ICurrentMember currentMember,
        CancellationToken cancellationToken)
    {
        var access = await currentMember.GetAccessAsync(cancellationToken);
        var canShare = access?.HasPermission(Permissions.OnlinePayments.Share) == true;
        var accounts = await accountRepository.ListOfOrganizationAsync(business.OrganizationId, cancellationToken);
        var current = accounts.FirstOrDefault(use => use.Account.Id == business.OnlinePaymentAccountId);
        var brandAccounts = canShare
            ? accounts.Where(use => use.Account.Id != business.OnlinePaymentAccountId).Select(ResponseOf).ToList()
            : [];

        return new OnlinePaymentsResponse(
            current?.Account.Status ?? OnlinePaymentStatus.NotConnected,
            OnlinePaymentCountries.IsAvailableIn(business.CountryCode),
            current is null ? null : ResponseOf(current),
            brandAccounts);
    }

    private static OnlinePaymentAccountResponse ResponseOf(OnlinePaymentAccountUse use) =>
        new(use.Account.Id, use.Account.Status, use.BranchNames);
}
