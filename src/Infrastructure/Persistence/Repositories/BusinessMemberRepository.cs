namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class BusinessMemberRepository(AppDbContext context) : IBusinessMemberRepository
{
    public void Add(BusinessMember member) => context.BusinessMembers.Add(member);

    public void Remove(BusinessMember member) => context.BusinessMembers.Remove(member);

    public async Task<IReadOnlyList<BusinessMember>> ListAsync(CancellationToken cancellationToken) =>
        await context.BusinessMembers.AsNoTracking().Include(member => member.User).ToListAsync(cancellationToken);

    public Task<BusinessMember?> GetForUpdateAsync(long memberId, CancellationToken cancellationToken) =>
        context.BusinessMembers.Include(member => member.User).FirstOrDefaultAsync(member => member.Id == memberId, cancellationToken);

    public Task<bool> IsAccountMemberAsync(Guid accountId, CancellationToken cancellationToken) =>
        context.BusinessMembers.AnyAsync(member => member.User.AccountId == accountId, cancellationToken);

    public async Task<IReadOnlyList<BusinessMember>> ListByCustomRoleAsync(long customRoleId, CancellationToken cancellationToken) =>
        await context.BusinessMembers.AsNoTracking().Where(member => member.CustomRoleId == customRoleId).ToListAsync(cancellationToken);

    public Task<bool> IsInstructorLinkedAsync(long instructorId, long? exceptMemberId, CancellationToken cancellationToken) =>
        context.BusinessMembers.AnyAsync(
            member => member.InstructorId == instructorId && member.Id != exceptMemberId,
            cancellationToken);

    public Task<Guid?> FindAccountIdByInstructorAsync(long instructorId, CancellationToken cancellationToken) =>
        context.BusinessMembers
            .Where(member => member.InstructorId == instructorId)
            .Select(member => member.User.AccountId)
            .FirstOrDefaultAsync(cancellationToken);
}
