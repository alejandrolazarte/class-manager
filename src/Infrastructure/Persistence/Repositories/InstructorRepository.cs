namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class InstructorRepository(AppDbContext context) : IInstructorRepository
{
    public void Add(Instructor instructor) => context.Instructors.Add(instructor);

    public Task<Instructor?> GetByIdAsync(Guid instructorId, CancellationToken cancellationToken) =>
        context.Instructors.AsNoTracking().Include(instructor => instructor.User).FirstOrDefaultAsync(instructor => instructor.Id == instructorId, cancellationToken);

    public Task<Instructor?> GetForUpdateAsync(Guid instructorId, CancellationToken cancellationToken) =>
        context.Instructors.Include(instructor => instructor.User).FirstOrDefaultAsync(instructor => instructor.Id == instructorId, cancellationToken);

    public Task<Instructor?> FindByNameAsync(string fullName, CancellationToken cancellationToken) =>
        context.Instructors.AsNoTracking().FirstOrDefaultAsync(instructor => instructor.User.FullName == fullName, cancellationToken);

    public async Task<IReadOnlyList<Instructor>> ListAllAsync(CancellationToken cancellationToken) =>
        await context.Instructors.AsNoTracking().Include(instructor => instructor.User)
            .OrderByDescending(instructor => instructor.IsActive)
            .ThenBy(instructor => instructor.User.FullName)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Instructor>> ListActiveAsync(CancellationToken cancellationToken) =>
        await context.Instructors.AsNoTracking().Include(instructor => instructor.User)
            .Where(instructor => instructor.IsActive)
            .OrderBy(instructor => instructor.User.FullName)
            .ToListAsync(cancellationToken);
}
