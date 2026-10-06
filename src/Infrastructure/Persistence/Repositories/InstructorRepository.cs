namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class InstructorRepository(AppDbContext context) : IInstructorRepository
{
    public void Add(Instructor instructor) => context.Instructors.Add(instructor);

    public Task<Instructor?> GetByIdAsync(Guid instructorId, CancellationToken cancellationToken) =>
        context.Instructors.AsNoTracking().FirstOrDefaultAsync(instructor => instructor.Id == instructorId, cancellationToken);

    public Task<Instructor?> GetForUpdateAsync(Guid instructorId, CancellationToken cancellationToken) =>
        context.Instructors.FirstOrDefaultAsync(instructor => instructor.Id == instructorId, cancellationToken);

    public async Task<IReadOnlyList<Instructor>> ListForUpdateInAnyBusinessByMemberUserAsync(Guid userId, CancellationToken cancellationToken) =>
        await context.Instructors
            .IgnoreQueryFilters()
            .Where(instructor => context.BusinessMembers
                .IgnoreQueryFilters()
                .Any(member => member.InstructorId == instructor.Id && member.UserId == userId))
            .ToListAsync(cancellationToken);

    public Task<Instructor?> FindByNameAsync(string fullName, CancellationToken cancellationToken) =>
        context.Instructors.AsNoTracking().FirstOrDefaultAsync(instructor => instructor.FullName == fullName, cancellationToken);

    public async Task<IReadOnlyList<Instructor>> ListAllAsync(CancellationToken cancellationToken) =>
        await context.Instructors.AsNoTracking()
            .OrderByDescending(instructor => instructor.IsActive)
            .ThenBy(instructor => instructor.FullName)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Instructor>> ListActiveAsync(CancellationToken cancellationToken) =>
        await context.Instructors.AsNoTracking()
            .Where(instructor => instructor.IsActive)
            .OrderBy(instructor => instructor.FullName)
            .ToListAsync(cancellationToken);
}
