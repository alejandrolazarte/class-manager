using System.Globalization;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record GetStudentAppNewsQuery : IQuery;

public sealed class GetStudentAppNewsUseCase(
    IStudentAppAccess studentAppAccess,
    IClientAccountRepository clientAccountRepository,
    IStudentRepository studentRepository,
    IEnrollmentRepository enrollmentRepository,
    IClassGroupRepository classGroupRepository,
    IClassSessionRepository sessionRepository,
    IClassFeedbackRepository feedbackRepository,
    IAnnouncementRepository announcementRepository,
    IOrderRepository orderRepository,
    IInstructorRepository instructorRepository,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider)
    : IUseCase<GetStudentAppNewsQuery, StudentAppNewsResponse>
{
    public const int AnnouncementDays = 60;
    public const int FeedbackDays = 30;
    public const int CancelledClassLookAheadDays = 14;
    public const int ItemLimit = 30;

    private const int OrderLimit = 20;
    private const string NoAccessMessage = "This account is not linked to a client.";
    private const string NoAccessCode = "student.no_access";
    private const string IdSeparator = ":";
    private const string IsoDateFormat = "yyyy-MM-dd";

    public async Task<Result<StudentAppNewsResponse>> ExecuteAsync(GetStudentAppNewsQuery command, CancellationToken cancellationToken)
    {
        var access = await studentAppAccess.GetAsync(cancellationToken);
        if (access is null)
        {
            return Result.Unauthorized<StudentAppNewsResponse>(NoAccessMessage, NoAccessCode);
        }

        var now = timeProvider.GetUtcNow();
        var seenAt = (await clientAccountRepository.FindByUserAsync(access.UserId, cancellationToken))?.NewsSeenAt;
        var today = await businessCalendar.TodayAsync(cancellationToken);
        var students = await studentRepository.ListByClientAsync(access.ClientId, cancellationToken);
        var studentIds = students.Select(student => student.Id).ToList();
        var studentNames = students.ToDictionary(student => student.Id, student => student.FullName);
        var instructorNames = (await instructorRepository.ListAllAsync(cancellationToken))
            .ToDictionary(instructor => instructor.Id, instructor => instructor.FullName);

        var items = new List<StudentAppNewsItemResponse>();
        StudentAppNewsItemResponse Item(
            StudentAppNewsKind kind,
            string sourceKey,
            DateTimeOffset occurredAt,
            string? title = null,
            string? body = null,
            IReadOnlyList<string>? studentFullNames = null,
            string? className = null,
            string? instructorFullName = null,
            DateOnly? classDate = null,
            Guid? orderId = null) =>
            new(
                string.Join(IdSeparator, kind.ToString(), sourceKey),
                kind,
                occurredAt,
                seenAt is null || occurredAt > seenAt,
                title,
                body,
                studentFullNames ?? [],
                className,
                instructorFullName,
                classDate,
                orderId);

        items.AddRange((await announcementRepository.ListPublishedSinceAsync(now.AddDays(-AnnouncementDays), cancellationToken))
            .Select(announcement => Item(
                StudentAppNewsKind.Announcement, announcement.Id.ToString(), announcement.PublishedAt, announcement.Title, announcement.Body)));

        items.AddRange((await orderRepository.ListAsync(new OrderSearchCriteria(access.ClientId, true, OrderLimit), null, cancellationToken))
            .Where(order => order.ReadyAt is not null)
            .Select(order => Item(StudentAppNewsKind.OrderReady, order.Id.ToString(), order.ReadyAt!.Value, orderId: order.Id)));

        items.AddRange((await feedbackRepository.ListUpdatedSinceAsync(studentIds, now.AddDays(-FeedbackDays), cancellationToken))
            .Select(feedback => Item(
                StudentAppNewsKind.InstructorFeedback,
                string.Join(IdSeparator, feedback.StudentId, feedback.Date.ToString(IsoDateFormat, CultureInfo.InvariantCulture)),
                feedback.UpdatedAt,
                body: feedback.Text,
                studentFullNames: [studentNames.GetValueOrDefault(feedback.StudentId) ?? string.Empty],
                className: feedback.ClassGroupName,
                instructorFullName: instructorNames.GetValueOrDefault(feedback.InstructorId),
                classDate: feedback.Date)));

        items.AddRange(await CancelledClassesAsync(
            students,
            today,
            (session, className, studentFullNames) => Item(
                StudentAppNewsKind.ClassCancelled,
                session.Id.ToString(),
                session.CancelledAt ?? session.CreatedAt,
                body: session.CancellationReason,
                studentFullNames: studentFullNames,
                className: className,
                classDate: session.Date),
            cancellationToken));

        var newestItems = items.OrderByDescending(item => item.OccurredAt).Take(ItemLimit).ToList();
        return new StudentAppNewsResponse(newestItems, newestItems.Count(item => item.IsUnread));
    }

    private async Task<IReadOnlyList<StudentAppNewsItemResponse>> CancelledClassesAsync(
        IReadOnlyList<Domain.Students.Student> students,
        DateOnly today,
        Func<Domain.Sessions.ClassSession, string, IReadOnlyList<string>, StudentAppNewsItemResponse> item,
        CancellationToken cancellationToken)
    {
        var cancelledSessions = (await sessionRepository.ListBetweenAsync(today, today.AddDays(CancelledClassLookAheadDays), cancellationToken))
            .Where(session => session.IsCancelled)
            .ToList();
        if (cancelledSessions.Count == 0)
        {
            return [];
        }

        var studentsBySession = new Dictionary<Guid, List<string>>();
        var classNames = new Dictionary<Guid, string>();
        foreach (var student in students)
        {
            foreach (var enrollment in await enrollmentRepository.ListCurrentByStudentAsync(student.Id, today, cancellationToken))
            {
                var classGroup = await classGroupRepository.GetByIdAsync(enrollment.ClassGroupId, cancellationToken);
                if (classGroup is null)
                {
                    continue;
                }

                foreach (var session in cancelledSessions.Where(session => session.ClassGroupId == enrollment.ClassGroupId
                    && session.Date >= enrollment.StartDate
                    && (enrollment.EndDate is not { } endDate || session.Date <= endDate)))
                {
                    classNames[session.Id] = classGroup.Name;
                    if (!studentsBySession.TryGetValue(session.Id, out var studentFullNames))
                    {
                        studentFullNames = [];
                        studentsBySession[session.Id] = studentFullNames;
                    }

                    studentFullNames.Add(student.FullName);
                }
            }
        }

        return
        [
            .. cancelledSessions
                .Where(session => studentsBySession.ContainsKey(session.Id))
                .Select(session => item(session, classNames[session.Id], studentsBySession[session.Id])),
        ];
    }
}
