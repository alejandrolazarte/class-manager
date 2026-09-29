namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class BusinessMemberRepository(AppDbContext context) : IBusinessMemberRepository
{
    public void Add(BusinessMember member) => context.BusinessMembers.Add(member);

    public void Remove(BusinessMember member) => context.BusinessMembers.Remove(member);

    public async Task<IReadOnlyList<BusinessMember>> ListAsync(CancellationToken cancellationToken) =>
        await context.BusinessMembers.AsNoTracking().ToListAsync(cancellationToken);

    public Task<BusinessMember?> GetForUpdateAsync(Guid memberId, CancellationToken cancellationToken) =>
        context.BusinessMembers.FirstOrDefaultAsync(member => member.Id == memberId, cancellationToken);

    public Task<bool> IsUserMemberAsync(Guid userId, CancellationToken cancellationToken) =>
        context.BusinessMembers.AnyAsync(member => member.UserId == userId, cancellationToken);

    public async Task<IReadOnlyList<BusinessMember>> ListByCustomRoleAsync(Guid customRoleId, CancellationToken cancellationToken) =>
        await context.BusinessMembers.AsNoTracking().Where(member => member.CustomRoleId == customRoleId).ToListAsync(cancellationToken);

    public Task<bool> IsInstructorLinkedAsync(Guid instructorId, Guid? exceptMemberId, CancellationToken cancellationToken) =>
        context.BusinessMembers.AnyAsync(
            member => member.InstructorId == instructorId && member.Id != exceptMemberId,
            cancellationToken);
}
