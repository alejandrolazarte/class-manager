using System.Globalization;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.PrivateLessons;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.UseCases.PrivateLessons;

public sealed record PrivateLessonStudentResponse(
    Guid StudentId,
    string StudentFullName,
    DateOnly? BirthDate,
    Guid ClientId,
    string ClientFullName,
    AttendanceStatus? Status);

public sealed record PrivateLessonResponse(
    Guid Id,
    Guid InstructorId,
    string InstructorFullName,
    DateOnly Date,
    string StartTime,
    string EndTime,
    int DurationMinutes,
    string? Location,
    string? Notes,
    bool IsCancelled,
    string? CancellationReason,
    Guid? SeriesId,
    bool CanTakeAttendance,
    IReadOnlyList<PrivateLessonStudentResponse> Students,
    bool IsTrial,
    decimal? TrialPrice)
{
    public static PrivateLessonResponse From(
        PrivateLesson lesson,
        string instructorFullName,
        IReadOnlyCollection<StudentSummary> studentSummaries,
        DateOnly today)
    {
        var summaries = studentSummaries.ToDictionary(summary => summary.Id);
        return new PrivateLessonResponse(
            lesson.Id,
            lesson.InstructorId,
            instructorFullName,
            lesson.Date,
            FormatTime(lesson.StartTime),
            FormatTime(lesson.EndTime),
            lesson.DurationMinutes,
            lesson.Location,
            lesson.Notes,
            lesson.IsCancelled,
            lesson.CancellationReason,
            lesson.SeriesId,
            !lesson.IsCancelled && lesson.Date <= today,
            [
                .. lesson.Students
                    .Select(lessonStudent =>
                    {
                        var summary = summaries.GetValueOrDefault(lessonStudent.StudentId);
                        return new PrivateLessonStudentResponse(
                            lessonStudent.StudentId,
                            summary?.FullName ?? string.Empty,
                            summary?.BirthDate,
                            summary?.ClientId ?? Guid.Empty,
                            summary?.ClientFullName ?? string.Empty,
                            lessonStudent.Status);
                    })
                    .OrderBy(student => student.StudentFullName, StringComparer.CurrentCultureIgnoreCase),
            ],
            lesson.IsTrial,
            lesson.TrialPrice);
    }

    public static string FormatTime(TimeOnly time) => time.ToString(ClassSchedule.TimeFormat, CultureInfo.InvariantCulture);
}
