namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class MeasurementGoalRepository(AppDbContext context) : IMeasurementGoalRepository
{
    public void Add(MeasurementGoal goal) => context.MeasurementGoals.Add(goal);

    public void Remove(MeasurementGoal goal) => context.MeasurementGoals.Remove(goal);

    public Task<MeasurementGoal?> FindForUpdateAsync(Guid studentId, Guid definitionId, CancellationToken cancellationToken) =>
        context.MeasurementGoals.FirstOrDefaultAsync(
            goal => goal.StudentId == studentId && goal.MeasurementDefinitionId == definitionId,
            cancellationToken);

    public async Task<IReadOnlyList<MeasurementGoal>> ListByStudentsAsync(IReadOnlyCollection<Guid> studentIds, CancellationToken cancellationToken) =>
        await context.MeasurementGoals.AsNoTracking()
            .Where(goal => studentIds.Contains(goal.StudentId))
            .ToListAsync(cancellationToken);
}
