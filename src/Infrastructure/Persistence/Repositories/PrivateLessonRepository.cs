namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class PrivateLessonRepository(AppDbContext context) : IPrivateLessonRepository
{
    public void Add(PrivateLesson lesson) => context.PrivateLessons.Add(lesson);

    public void Remove(PrivateLesson lesson) => context.PrivateLessons.Remove(lesson);

    public Task<PrivateLesson?> GetByIdAsync(Guid privateLessonId, CancellationToken cancellationToken) =>
        context.PrivateLessons.AsNoTracking()
            .Include(lesson => lesson.Students)
            .FirstOrDefaultAsync(lesson => lesson.Id == privateLessonId, cancellationToken);

    public Task<PrivateLesson?> GetForUpdateAsync(Guid privateLessonId, CancellationToken cancellationToken) =>
        context.PrivateLessons
            .Include(lesson => lesson.Students)
            .FirstOrDefaultAsync(lesson => lesson.Id == privateLessonId, cancellationToken);

    public async Task<IReadOnlyList<PrivateLesson>> ListBetweenAsync(DateOnly firstDate, DateOnly lastDate, CancellationToken cancellationToken) =>
        await context.PrivateLessons.AsNoTracking()
            .Include(lesson => lesson.Students)
            .Where(lesson => lesson.Date >= firstDate && lesson.Date <= lastDate)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PrivateLesson>> ListByInstructorOnDatesAsync(
        Guid instructorId,
        IReadOnlyCollection<DateOnly> dates,
        CancellationToken cancellationToken) =>
        await context.PrivateLessons.AsNoTracking()
            .Where(lesson => lesson.InstructorId == instructorId && dates.Contains(lesson.Date))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PrivateLesson>> ListByInstructorFromAsync(
        Guid instructorId,
        DateOnly firstDate,
        CancellationToken cancellationToken) =>
        await context.PrivateLessons.AsNoTracking()
            .Where(lesson => lesson.InstructorId == instructorId && lesson.Date >= firstDate && !lesson.IsCancelled)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PaidTrialLesson>> ListPaidTrialsByClientAsync(Guid clientId, CancellationToken cancellationToken) =>
        await (
            from lesson in context.PrivateLessons.AsNoTracking()
            where lesson.IsTrial && lesson.TrialPrice != null && !lesson.IsCancelled
            join lessonStudent in context.PrivateLessonStudents.AsNoTracking() on lesson.Id equals lessonStudent.PrivateLessonId
            join student in context.Students.AsNoTracking() on lessonStudent.StudentId equals student.Id
            where student.ClientId == clientId
            orderby lesson.Date
            select new PaidTrialLesson(lesson.Id, lesson.Date, student.FullName, lesson.TrialPrice!.Value))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ClientAttendedClass>> ListAttendedClassesByClientsAsync(
        IReadOnlyCollection<Guid> clientIds,
        CancellationToken cancellationToken)
    {
        if (clientIds.Count == 0)
        {
            return [];
        }

        var rows = await (
            from lessonStudent in context.PrivateLessonStudents.AsNoTracking()
            where lessonStudent.Status == AttendanceStatus.Present
            join student in context.Students.AsNoTracking() on lessonStudent.StudentId equals student.Id
            where clientIds.Contains(student.ClientId)
            join lesson in context.PrivateLessons.AsNoTracking() on lessonStudent.PrivateLessonId equals lesson.Id
            where !lesson.IsTrial
            join instructor in context.Instructors.AsNoTracking() on lesson.InstructorId equals instructor.Id
            select new { student.ClientId, lesson.Date, StudentFullName = student.FullName, InstructorFullName = instructor.FullName })
            .ToListAsync(cancellationToken);

        return
        [
            .. rows.Select(row => new ClientAttendedClass(
                row.ClientId,
                new AttendedClass(row.Date, row.StudentFullName, row.InstructorFullName, IsPrivateLesson: true))),
        ];
    }
}
