using System.Globalization;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.UseCases.Enrollments;

public sealed record ListStudentEnrollmentsQuery(Guid StudentId);

public sealed record StudentEnrollmentResponse(
    Guid EnrollmentId,
    Guid ClassGroupId,
    string ClassGroupName,
    IReadOnlyList<DayOfWeek> Weekdays,
    string StartTime,
    string EndTime,
    string InstructorFullName,
    DateOnly StartDate,
    DateOnly? EndDate);

public sealed class ListStudentEnrollmentsUseCase(
    IStudentRepository studentRepository,
    IClassGroupRepository classGroupRepository,
    IInstructorRepository instructorRepository,
    IEnrollmentRepository enrollmentRepository,
    IBusinessCalendarService businessCalendar,
    IClientRepository clientRepository,
    IAccessScopes accessScopes)
    : IUseCase<ListStudentEnrollmentsQuery, IReadOnlyList<StudentEnrollmentResponse>>
{
    private const string StudentNotFoundMessage = "The student does not exist.";

    public async Task<Result<IReadOnlyList<StudentEnrollmentResponse>>> ExecuteAsync(ListStudentEnrollmentsQuery command, CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetSummaryByIdAsync(command.StudentId, cancellationToken);
        if (student is null || !await AccessRules.CanReachClientsAsync(accessScopes, clientRepository, [student.ClientId], cancellationToken))
        {
            return Result.NotFound<IReadOnlyList<StudentEnrollmentResponse>>(StudentNotFoundMessage, StudentErrorCodes.NotFound);
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var enrollments = await enrollmentRepository.ListCurrentByStudentAsync(student.Id, today, cancellationToken);
        var classGroups = (await classGroupRepository.ListAllAsync(cancellationToken)).ToDictionary(classGroup => classGroup.Id);
        var instructorNames = (await instructorRepository.ListAllAsync(cancellationToken))
            .ToDictionary(instructor => instructor.Id, instructor => instructor.FullName);

        return Result.Success<IReadOnlyList<StudentEnrollmentResponse>>(
        [
            .. enrollments
                .Where(enrollment => classGroups.ContainsKey(enrollment.ClassGroupId))
                .Select(enrollment =>
                {
                    var classGroup = classGroups[enrollment.ClassGroupId];
                    var schedule = classGroup.Schedule;
                    return new StudentEnrollmentResponse(
                        enrollment.Id,
                        classGroup.Id,
                        classGroup.Name,
                        schedule.Days,
                        schedule.StartTime.ToString(ClassSchedule.TimeFormat, CultureInfo.InvariantCulture),
                        schedule.EndTime.ToString(ClassSchedule.TimeFormat, CultureInfo.InvariantCulture),
                        instructorNames.GetValueOrDefault(classGroup.InstructorId, string.Empty),
                        enrollment.StartDate,
                        enrollment.EndDate);
                })
                .OrderBy(enrollment => enrollment.StartTime, StringComparer.Ordinal),
        ]);
    }
}
