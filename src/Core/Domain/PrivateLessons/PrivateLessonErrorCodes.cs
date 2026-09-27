namespace ClassManager.Core.Domain.PrivateLessons;

public static class PrivateLessonErrorCodes
{
    public const string NotFound = "private_lesson.not_found";
    public const string StudentNotInLesson = "private_lesson.student_not_in_lesson";
    public const string ConflictingDateDetail = "date";
    public const string ConflictingPrivateLessonIdDetail = "privateLessonId";
}
