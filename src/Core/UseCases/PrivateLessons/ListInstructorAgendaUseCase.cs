using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Core.UseCases.PrivateLessons;

public sealed record ListInstructorAgendaQuery(long? InstructorId, DateOnly? Date, long? IgnoredPrivateLessonId = null) : IQuery;

public enum InstructorAgendaSlotKind
{
    ClassGroup,
    PrivateLesson,
}

public sealed record InstructorAgendaSlotResponse(
    string StartTime,
    int DurationMinutes,
    InstructorAgendaSlotKind Kind,
    string? ClassGroupName);

public sealed class ListInstructorAgendaUseCase(
    IInstructorRepository instructorRepository,
    IClassGroupRepository classGroupRepository,
    IClassSessionRepository sessionRepository,
    IPrivateLessonRepository privateLessonRepository,
    IAccessScopes accessScopes)
    : IUseCase<ListInstructorAgendaQuery, IReadOnlyList<InstructorAgendaSlotResponse>>
{
    private const string DateRequiredMessage = "Choose a date.";

    public async Task<Result<IReadOnlyList<InstructorAgendaSlotResponse>>> ExecuteAsync(
        ListInstructorAgendaQuery command,
        CancellationToken cancellationToken)
    {
        if (command.Date is not { } date)
        {
            return Result.Validation<IReadOnlyList<InstructorAgendaSlotResponse>>(
                DateRequiredMessage, fieldName: nameof(ListInstructorAgendaQuery.Date));
        }

        var instructor = command.InstructorId is { } instructorId
            ? await instructorRepository.GetByIdAsync(instructorId, cancellationToken)
            : null;
        if (instructor is null)
        {
            return InstructorFailures.NotFound();
        }

        var scope = await accessScopes.ForInstructorsAsync(Permissions.PrivateLessons.ManageAll, cancellationToken);
        if (!scope.Includes(instructor.Id))
        {
            return AccessRules.NotYours();
        }

        var taughtClasses = await InstructorAgendaRules.ListTaughtClassesAsync(
            new InstructorAgendaRepositories(classGroupRepository, sessionRepository, privateLessonRepository),
            instructor.Id,
            [date],
            cancellationToken);
        var privateLessons = await privateLessonRepository.ListByInstructorOnDatesAsync(instructor.Id, [date], cancellationToken);

        var classSlots = taughtClasses.Select(taughtClass => new InstructorAgendaSlotResponse(
            PrivateLessonResponse.FormatTime(taughtClass.Schedule.StartTime),
            taughtClass.Schedule.DurationMinutes,
            InstructorAgendaSlotKind.ClassGroup,
            taughtClass.ClassGroup.Name));
        var lessonSlots = privateLessons
            .Where(lesson => lesson.Date == date && !lesson.IsCancelled && lesson.Id != command.IgnoredPrivateLessonId)
            .Select(lesson => new InstructorAgendaSlotResponse(
                PrivateLessonResponse.FormatTime(lesson.StartTime),
                lesson.DurationMinutes,
                InstructorAgendaSlotKind.PrivateLesson,
                null));

        return Result.Success<IReadOnlyList<InstructorAgendaSlotResponse>>(
            [.. classSlots.Concat(lessonSlots).OrderBy(slot => slot.StartTime, StringComparer.Ordinal)]);
    }
}
