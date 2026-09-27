using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.PrivateLessons;

namespace ClassManager.Core.UseCases.PrivateLessons;

internal static class PrivateLessonRules
{
    private const string NotFoundMessage = "The private lesson does not exist.";

    public static ResultError NotFound() => new(PrivateLessonErrorCodes.NotFound, NotFoundMessage, ErrorKind.NotFound);

    public static async Task<PrivateLessonResponse> ToResponseAsync(
        PrivateLesson lesson,
        IInstructorRepository instructorRepository,
        IStudentRepository studentRepository,
        DateOnly today,
        CancellationToken cancellationToken)
    {
        var instructor = await instructorRepository.GetByIdAsync(lesson.InstructorId, cancellationToken);
        var students = await studentRepository.ListSummariesByIdsAsync(
            [.. lesson.Students.Select(lessonStudent => lessonStudent.StudentId)], cancellationToken);
        return PrivateLessonResponse.From(lesson, instructor?.FullName ?? string.Empty, students, today);
    }
}
