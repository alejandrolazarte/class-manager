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
        CancellationToken cancellationToken) =>
        [
            .. (await ListAsync(studentIds, since: null, cancellationToken))
                .GroupBy(feedback => feedback.StudentId)
                .Select(group => group.OrderByDescending(feedback => feedback.Date).ThenByDescending(feedback => feedback.UpdatedAt).First()),
        ];

    public Task<IReadOnlyList<StudentFeedback>> ListUpdatedSinceAsync(
        IReadOnlyCollection<Guid> studentIds,
        DateTimeOffset since,
        CancellationToken cancellationToken) =>
        ListAsync(studentIds, since, cancellationToken);

    private async Task<IReadOnlyList<StudentFeedback>> ListAsync(
        IReadOnlyCollection<Guid> studentIds,
        DateTimeOffset? since,
        CancellationToken cancellationToken)
    {
        if (studentIds.Count == 0)
        {
            return [];
        }

        var rows = await (
            from feedback in context.ClassFeedbacks.AsNoTracking()
            where studentIds.Contains(feedback.StudentId) && (since == null || feedback.UpdatedAt >= since)
            join session in context.ClassSessions.AsNoTracking() on feedback.ClassSessionId equals session.Id
            join classGroup in context.ClassGroups.AsNoTracking() on session.ClassGroupId equals classGroup.Id
            select new { feedback.StudentId, session.Date, ClassGroupName = classGroup.Name, feedback.InstructorId, feedback.Text, feedback.UpdatedAt })
            .ToListAsync(cancellationToken);

        return [.. rows.Select(row => new StudentFeedback(row.StudentId, row.Date, row.ClassGroupName, row.InstructorId, row.Text, row.UpdatedAt))];
    }
}
