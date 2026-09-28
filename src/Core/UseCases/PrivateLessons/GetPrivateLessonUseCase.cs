using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;

namespace ClassManager.Core.UseCases.PrivateLessons;

public sealed record GetPrivateLessonQuery(Guid PrivateLessonId);

public sealed class GetPrivateLessonUseCase(
    IPrivateLessonRepository privateLessonRepository,
    IInstructorRepository instructorRepository,
    IStudentRepository studentRepository,
    IBusinessCalendarService businessCalendar,
    IAccessScopes accessScopes)
    : IUseCase<GetPrivateLessonQuery, PrivateLessonResponse>
{
    public async Task<Result<PrivateLessonResponse>> ExecuteAsync(GetPrivateLessonQuery command, CancellationToken cancellationToken)
    {
        var lesson = await privateLessonRepository.GetByIdAsync(command.PrivateLessonId, cancellationToken);
        var scope = await accessScopes.ForInstructorsAsync(Permissions.PrivateLessons.ViewAll, cancellationToken);
        if (lesson is null || !scope.Includes(lesson.InstructorId))
        {
            return PrivateLessonRules.NotFound();
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        return await PrivateLessonRules.ToResponseAsync(lesson, instructorRepository, studentRepository, today, cancellationToken);
    }
}
