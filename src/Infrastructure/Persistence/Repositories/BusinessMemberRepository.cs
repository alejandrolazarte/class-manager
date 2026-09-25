namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class BusinessMemberRepository(AppDbContext context) : IBusinessMemberRepository
{
    public void Add(BusinessMember member) => context.BusinessMembers.Add(member);

    public Task<BusinessMember?> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken) =>
        context.BusinessMembers
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(member => member.UserId == userId)
            .OrderBy(member => member.Id)
            .FirstOrDefaultAsync(cancellationToken);
}
