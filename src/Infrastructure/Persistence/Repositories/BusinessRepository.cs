namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class BusinessRepository(AppDbContext context, ITenantContext tenantContext) : IBusinessRepository
{
    private Business? _currentBusiness;
    private bool _isCurrentBusinessRead;
    private bool _mayChangeInThisRequest;

    public void Add(Business business)
    {
        _mayChangeInThisRequest = true;
        context.Businesses.Add(business);
    }

    public async Task<Business?> GetCurrentAsync(CancellationToken cancellationToken)
    {
        if (_mayChangeInThisRequest)
        {
            return await ReadCurrentAsync(cancellationToken);
        }

        if (!_isCurrentBusinessRead)
        {
            _currentBusiness = await ReadCurrentAsync(cancellationToken);
            _isCurrentBusinessRead = true;
        }

        return _currentBusiness;
    }

    public Task<Business?> GetCurrentForUpdateAsync(CancellationToken cancellationToken)
    {
        _mayChangeInThisRequest = true;
        return context.Businesses.FirstOrDefaultAsync(business => business.Id == tenantContext.TenantId, cancellationToken);
    }

    public Task<bool> IsSlugTakenAsync(string slug, CancellationToken cancellationToken) =>
        context.Businesses.AnyAsync(business => business.Slug == slug, cancellationToken);

    private Task<Business?> ReadCurrentAsync(CancellationToken cancellationToken) =>
        context.Businesses.AsNoTracking().FirstOrDefaultAsync(business => business.Id == tenantContext.TenantId, cancellationToken);
}
