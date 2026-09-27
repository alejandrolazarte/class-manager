using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.PrivateLessons;

public sealed class PrivateLesson : ITenantOwned
{
    public const int MinimumStudents = 1;
    public const int MaximumStudents = 4;
    public const int LocationMaxLength = ClassGroup.LocationMaxLength;
    public const int NotesMaxLength = 500;
    public const int CancellationReasonMaxLength = ClassSession.CancellationReasonMaxLength;

    private const string StudentCountMessage = "A private lesson has between 1 and 4 different students.";
    private const string LocationLengthMessage = "Location must be at most 80 characters.";
    private const string NotesLengthMessage = "Notes must be at most 500 characters.";
    private const string CancellationReasonLengthMessage = "The reason must be at most 200 characters.";
    private const string HasAttendanceMessage = "Attendance was already taken for this lesson.";
    private const string CancelledMessage = "The lesson is cancelled.";
    private const string StudentNotInLessonMessage = "The student isn't in this lesson.";
    private const string StudentIdsFieldName = "StudentIds";
    private const int MinutesPerHour = 60;

    private readonly List<PrivateLessonStudent> _students = [];

    private PrivateLesson()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid InstructorId { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public int DurationMinutes { get; private set; }
    public string? Location { get; private set; }
    public string? Notes { get; private set; }
    public bool IsCancelled { get; private set; }
    public string? CancellationReason { get; private set; }
    public Guid? SeriesId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public IReadOnlyList<PrivateLessonStudent> Students => _students;

    public TimeOnly EndTime => StartTime.AddMinutes(DurationMinutes);
    public bool HasAttendance => _students.Any(student => student.Status is not null);

    public static Result<PrivateLesson> Create(
        Guid instructorId,
        DateOnly date,
        ClassSchedule schedule,
        IReadOnlyCollection<Guid> studentIds,
        string? location,
        string? notes,
        Guid? seriesId,
        DateTimeOffset createdAt)
    {
        var distinctStudentIds = studentIds.Distinct().ToList();
        if (distinctStudentIds.Count != studentIds.Count || studentIds.Count is < MinimumStudents or > MaximumStudents)
        {
            return Result.Validation<PrivateLesson>(StudentCountMessage, fieldName: StudentIdsFieldName);
        }

        var lesson = new PrivateLesson
        {
            Id = Guid.CreateVersion7(),
            SeriesId = seriesId,
            CreatedAt = createdAt.ToUniversalTime(),
        };
        var details = lesson.ApplyDetails(instructorId, date, schedule, location, notes);
        if (details.IsFailure)
        {
            return details.Error!;
        }

        lesson._students.AddRange(distinctStudentIds.Select(studentId => PrivateLessonStudent.Create(lesson.Id, studentId)));
        return lesson;
    }

    public Result Reschedule(Guid instructorId, DateOnly date, ClassSchedule schedule, string? location, string? notes)
    {
        if (HasAttendance)
        {
            return Result.Conflict(HasAttendanceMessage, SessionErrorCodes.HasAttendance);
        }

        return ApplyDetails(instructorId, date, schedule, location, notes);
    }

    public Result Cancel(string? reason)
    {
        if (HasAttendance)
        {
            return Result.Conflict(HasAttendanceMessage, SessionErrorCodes.HasAttendance);
        }

        var trimmedReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        if (trimmedReason?.Length > CancellationReasonMaxLength)
        {
            return Result.Validation(CancellationReasonLengthMessage, fieldName: nameof(CancellationReason));
        }

        IsCancelled = true;
        CancellationReason = trimmedReason;
        return Result.Success();
    }

    public void Restore()
    {
        IsCancelled = false;
        CancellationReason = null;
    }

    public Result Mark(Guid studentId, AttendanceStatus? status)
    {
        if (IsCancelled)
        {
            return Result.Conflict(CancelledMessage, SessionErrorCodes.Cancelled);
        }

        var lessonStudent = _students.FirstOrDefault(student => student.StudentId == studentId);
        if (lessonStudent is null)
        {
            return Result.Validation(StudentNotInLessonMessage, PrivateLessonErrorCodes.StudentNotInLesson, nameof(PrivateLessonStudent.StudentId));
        }

        lessonStudent.Mark(status);
        return Result.Success();
    }

    public bool OverlapsWith(DateOnly date, TimeOnly startTime, int durationMinutes)
    {
        var startMinute = MinuteOfDay(StartTime);
        var otherStartMinute = MinuteOfDay(startTime);
        return !IsCancelled
            && Date == date
            && startMinute < otherStartMinute + durationMinutes
            && otherStartMinute < startMinute + DurationMinutes;
    }

    private static int MinuteOfDay(TimeOnly time) => (time.Hour * MinutesPerHour) + time.Minute;

    private Result ApplyDetails(Guid instructorId, DateOnly date, ClassSchedule schedule, string? location, string? notes)
    {
        var trimmedLocation = string.IsNullOrWhiteSpace(location) ? null : location.Trim();
        if (trimmedLocation?.Length > LocationMaxLength)
        {
            return Result.Validation(LocationLengthMessage, fieldName: nameof(Location));
        }

        var trimmedNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        if (trimmedNotes?.Length > NotesMaxLength)
        {
            return Result.Validation(NotesLengthMessage, fieldName: nameof(Notes));
        }

        InstructorId = instructorId;
        Date = date;
        StartTime = schedule.StartTime;
        DurationMinutes = schedule.DurationMinutes;
        Location = trimmedLocation;
        Notes = trimmedNotes;
        return Result.Success();
    }
}
