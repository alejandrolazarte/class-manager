using ClassManager.Core.Domain.OnlinePayments;

namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class OnlinePaymentAccountRepository(AppDbContext context) : IOnlinePaymentAccountRepository
{
    public Task<OnlinePaymentAccount?> FindAsync(long organizationId, long accountId, CancellationToken cancellationToken) =>
        context.OnlinePaymentAccounts.SingleOrDefaultAsync(
            account => account.Id == accountId && account.OrganizationId == organizationId,
            cancellationToken);

    public Task<OnlinePaymentAccount?> FindCreatedForAsync(long businessId, CancellationToken cancellationToken) =>
        context.OnlinePaymentAccounts.SingleOrDefaultAsync(account => account.CreatedForBusinessId == businessId, cancellationToken);

    public Task<OnlinePaymentAccount?> FindByExternalIdAsync(string externalAccountId, CancellationToken cancellationToken) =>
        context.OnlinePaymentAccounts.SingleOrDefaultAsync(account => account.ExternalAccountId == externalAccountId, cancellationToken);

    public async Task<IReadOnlyList<OnlinePaymentAccountUse>> ListOfOrganizationAsync(long organizationId, CancellationToken cancellationToken)
    {
        var accounts = await context.OnlinePaymentAccounts
            .Where(account => account.OrganizationId == organizationId)
            .OrderBy(account => account.Id)
            .ToListAsync(cancellationToken);
        var branchesByAccountId = (await context.Businesses
                .Where(business => business.OrganizationId == organizationId && business.OnlinePaymentAccountId != null)
                .Select(business => new { AccountId = business.OnlinePaymentAccountId!.Value, business.Name })
                .ToListAsync(cancellationToken))
            .ToLookup(branch => branch.AccountId, branch => branch.Name);

        return
        [
            .. accounts.Select(account => new OnlinePaymentAccountUse(
                account,
                [.. branchesByAccountId[account.Id].Order(StringComparer.CurrentCultureIgnoreCase)])),
        ];
    }

    public void Add(OnlinePaymentAccount account) => context.OnlinePaymentAccounts.Add(account);
}
