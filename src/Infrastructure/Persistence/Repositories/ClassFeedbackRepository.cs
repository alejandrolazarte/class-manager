namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class ClassFeedbackRepository(AppDbContext context) : IClassFeedbackRepository
{
    public void Add(ClassFeedback feedback) => context.ClassFeedbacks.Add(feedback);

    public void Remove(ClassFeedback feedback) => context.ClassFeedbacks.Remove(feedback);

    public Task<ClassFeedback?> FindForUpdateAsync(Guid classSessionId, Guid studentId, CancellationToken cancellationToken) =>
        context.ClassFeedbacks.FirstOrDefaultAsync(
            feedback => feedback.ClassSessionId == classSessionId && feedback.StudentId == studentId,
            cancellationToken);

    public async Task<IReadOnlyList<ClassFeedback>> ListBySessionAsync(Guid classSessionId, CancellationToken cancellationToken) =>
        await context.ClassFeedbacks.AsNoTracking()
            .Where(feedback => feedback.ClassSessionId == classSessionId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<StudentFeedback>> ListLatestByStudentsAsync(
        IReadOnlyCollection<Guid> studentIds,
        CancellationToken cancellationToken)
    {
        if (studentIds.Count == 0)
        {
            return [];
        }

        var rows = await (
            from feedback in context.ClassFeedbacks.AsNoTracking()
            where studentIds.Contains(feedback.StudentId)
            join session in context.ClassSessions.AsNoTracking() on feedback.ClassSessionId equals session.Id
            join classGroup in context.ClassGroups.AsNoTracking() on session.ClassGroupId equals classGroup.Id
            select new { feedback.StudentId, session.Date, ClassGroupName = classGroup.Name, feedback.InstructorId, feedback.Text, feedback.UpdatedAt })
            .ToListAsync(cancellationToken);

        return
        [
            .. rows
                .GroupBy(row => row.StudentId)
                .Select(group => group.OrderByDescending(row => row.Date).ThenByDescending(row => row.UpdatedAt).First())
                .Select(row => new StudentFeedback(row.StudentId, row.Date, row.ClassGroupName, row.InstructorId, row.Text)),
        ];
    }
}
