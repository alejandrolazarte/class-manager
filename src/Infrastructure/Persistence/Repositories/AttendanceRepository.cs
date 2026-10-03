namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class AttendanceRepository(AppDbContext context) : IAttendanceRepository
{
    public void Add(Attendance attendance) => context.Attendances.Add(attendance);

    public void Remove(Attendance attendance) => context.Attendances.Remove(attendance);

    public Task<Attendance?> FindForUpdateAsync(Guid classSessionId, Guid studentId, CancellationToken cancellationToken) =>
        context.Attendances.FirstOrDefaultAsync(
            attendance => attendance.ClassSessionId == classSessionId && attendance.StudentId == studentId,
            cancellationToken);

    public async Task<IReadOnlyList<Attendance>> ListBySessionAsync(Guid classSessionId, CancellationToken cancellationToken) =>
        await context.Attendances.AsNoTracking()
            .Where(attendance => attendance.ClassSessionId == classSessionId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyDictionary<Guid, AttendanceCount>> CountBySessionsAsync(
        IReadOnlyCollection<Guid> classSessionIds,
        CancellationToken cancellationToken)
    {
        if (classSessionIds.Count == 0)
        {
            return new Dictionary<Guid, AttendanceCount>();
        }

        var counts = await context.Attendances.AsNoTracking()
            .Where(attendance => classSessionIds.Contains(attendance.ClassSessionId))
            .GroupBy(attendance => attendance.ClassSessionId)
            .Select(group => new
            {
                ClassSessionId = group.Key,
                Present = group.Count(attendance => attendance.Status == AttendanceStatus.Present),
                Absent = group.Count(attendance => attendance.Status == AttendanceStatus.Absent),
            })
            .ToListAsync(cancellationToken);

        return counts.ToDictionary(count => count.ClassSessionId, count => new AttendanceCount(count.Present, count.Absent));
    }

    public async Task<IReadOnlyList<ClientAttendedClass>> ListAttendedClassesByClientsAsync(
        IReadOnlyCollection<Guid> clientIds,
        CancellationToken cancellationToken)
    {
        if (clientIds.Count == 0)
        {
            return [];
        }

        var rows = await (
            from attendance in context.Attendances.AsNoTracking()
            let isPackBooking = context.PackBookings.Any(booking =>
                booking.ClassSessionId == attendance.ClassSessionId && booking.StudentId == attendance.StudentId)
            where attendance.Status == AttendanceStatus.Present || isPackBooking
            join student in context.Students.AsNoTracking() on attendance.StudentId equals student.Id
            where clientIds.Contains(student.ClientId)
            join session in context.ClassSessions.AsNoTracking() on attendance.ClassSessionId equals session.Id
            join classGroup in context.ClassGroups.AsNoTracking() on session.ClassGroupId equals classGroup.Id
            select new { student.ClientId, session.Date, StudentFullName = student.FullName, ClassGroupName = classGroup.Name, IsPackBooking = isPackBooking })
            .ToListAsync(cancellationToken);

        return [.. rows.Select(row => new ClientAttendedClass(
            row.ClientId, new AttendedClass(row.Date, row.StudentFullName, row.ClassGroupName, IsPackBooking: row.IsPackBooking)))];
    }

    public async Task<IReadOnlyList<StudentAttendanceMark>> ListMarksByStudentsAsync(
        IReadOnlyCollection<Guid> studentIds,
        DateOnly firstDate,
        DateOnly today,
        CancellationToken cancellationToken)
    {
        if (studentIds.Count == 0)
        {
            return [];
        }

        var notices = (await (
            from notice in context.AbsenceNotices.AsNoTracking()
            where studentIds.Contains(notice.StudentId)
            join session in context.ClassSessions.AsNoTracking() on notice.ClassSessionId equals session.Id
            where session.Date >= firstDate && !session.IsCancelled
            select new { notice.StudentId, notice.ClassSessionId, notice.KeepsStreak, session.Date })
            .ToListAsync(cancellationToken))
            .ToDictionary(notice => (notice.StudentId, notice.ClassSessionId));
        var groupMarks = await (
            from attendance in context.Attendances.AsNoTracking()
            where studentIds.Contains(attendance.StudentId)
            join session in context.ClassSessions.AsNoTracking() on attendance.ClassSessionId equals session.Id
            where session.Date >= firstDate
            select new { attendance.StudentId, attendance.ClassSessionId, session.Date, attendance.Status })
            .ToListAsync(cancellationToken);
        var recordedSessions = groupMarks.Select(mark => (mark.StudentId, mark.ClassSessionId)).ToHashSet();
        var privateLessonMarks = await (
            from lessonStudent in context.PrivateLessonStudents.AsNoTracking()
            where studentIds.Contains(lessonStudent.StudentId) && lessonStudent.Status != null
            join lesson in context.PrivateLessons.AsNoTracking() on lessonStudent.PrivateLessonId equals lesson.Id
            where lesson.Date >= firstDate && !lesson.IsCancelled
            select new { lessonStudent.StudentId, lesson.Date, Status = lessonStudent.Status!.Value })
            .ToListAsync(cancellationToken);

        return
        [
            .. groupMarks.Select(row => new StudentAttendanceMark(
                row.StudentId,
                new AttendanceMark(row.Date, row.Status, IsExcused: notices.TryGetValue((row.StudentId, row.ClassSessionId), out var notice) && notice.KeepsStreak))),
            .. notices.Values
                .Where(notice => !recordedSessions.Contains((notice.StudentId, notice.ClassSessionId)))
                .Where(notice => notice.KeepsStreak || notice.Date < today)
                .Select(notice => new StudentAttendanceMark(
                    notice.StudentId, new AttendanceMark(notice.Date, AttendanceStatus.Absent, IsExcused: notice.KeepsStreak))),
            .. privateLessonMarks.Select(row => new StudentAttendanceMark(row.StudentId, new AttendanceMark(row.Date, row.Status))),
        ];
    }

    public async Task<IReadOnlyDictionary<Guid, int>> CountAttendedClassesByStudentsAsync(
        IReadOnlyCollection<Guid> studentIds,
        CancellationToken cancellationToken)
    {
        if (studentIds.Count == 0)
        {
            return new Dictionary<Guid, int>();
        }

        var groupCounts = await context.Attendances.AsNoTracking()
            .Where(attendance => studentIds.Contains(attendance.StudentId) && attendance.Status == AttendanceStatus.Present)
            .GroupBy(attendance => attendance.StudentId)
            .Select(group => new { StudentId = group.Key, Count = group.Count() })
            .ToListAsync(cancellationToken);
        var privateLessonCounts = await (
            from lessonStudent in context.PrivateLessonStudents.AsNoTracking()
            where studentIds.Contains(lessonStudent.StudentId) && lessonStudent.Status == AttendanceStatus.Present
            join lesson in context.PrivateLessons.AsNoTracking() on lessonStudent.PrivateLessonId equals lesson.Id
            where !lesson.IsCancelled
            group lessonStudent by lessonStudent.StudentId into studentLessons
            select new { StudentId = studentLessons.Key, Count = studentLessons.Count() })
            .ToListAsync(cancellationToken);

        return groupCounts.Concat(privateLessonCounts)
            .GroupBy(count => count.StudentId)
            .ToDictionary(group => group.Key, group => group.Sum(count => count.Count));
    }
}
