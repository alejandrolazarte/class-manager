using System.Globalization;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Enrollments;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record GetStudentAppNewsQuery : IQuery;

public sealed class GetStudentAppNewsUseCase(
    IStudentAppFamilyService studentAppFamily,
    IClientAccountRepository clientAccountRepository,
    IEnrollmentRepository enrollmentRepository,
    IClassGroupRepository classGroupRepository,
    IClassSessionRepository sessionRepository,
    IClassFeedbackRepository feedbackRepository,
    IAnnouncementRepository announcementRepository,
    IOrderRepository orderRepository,
    IInstructorRepository instructorRepository,
    IDocumentStorageService documentStorage,
    TimeProvider timeProvider)
    : IUseCase<GetStudentAppNewsQuery, StudentAppNewsResponse>
{
    public const int FeedbackDays = 30;
    public const int ClassChangeLookAheadDays = 14;
    public const int ItemLimit = 30;

    private const int OrderLimit = 20;
    private const string NoAccessMessage = "This account is not linked to a client.";
    private const string NoAccessCode = "student.no_access";
    private const string IdSeparator = ":";
    private const string IsoDateFormat = "yyyy-MM-dd";

    public async Task<Result<StudentAppNewsResponse>> ExecuteAsync(GetStudentAppNewsQuery command, CancellationToken cancellationToken)
    {
        var family = await studentAppFamily.GetAsync(cancellationToken);
        if (family is null)
        {
            return Result.Unauthorized<StudentAppNewsResponse>(NoAccessMessage, NoAccessCode);
        }

        var access = family.Access;

        var now = timeProvider.GetUtcNow();
        var seenAt = (await clientAccountRepository.FindByAccountAsync(access.AccountId, cancellationToken))?.NewsSeenAt;
        var today = family.Today;
        var students = family.Students;
        var studentIds = students.Select(student => student.Id).ToList();
        var studentNames = students.ToDictionary(student => student.Id, student => student.User.FullName);
        var instructorNames = (await instructorRepository.ListAllAsync(cancellationToken))
            .ToDictionary(instructor => instructor.Id, instructor => instructor.User.FullName);

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
            long? orderId = null,
            string? classStartTime = null) =>
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
                orderId,
                classStartTime);

        var currentEnrollments = await enrollmentRepository.ListCurrentByStudentsAsync(studentIds, today, cancellationToken);
        var enrolledClassGroupIds = StudentAnnouncements.EnrolledClassGroupIds(currentEnrollments);
        var enrolledClassGroups = (await classGroupRepository.ListByIdsAsync(enrolledClassGroupIds, cancellationToken))
            .ToDictionary(classGroup => classGroup.Id);
        var enrolledClassGroupNames = enrolledClassGroups.ToDictionary(entry => entry.Key, entry => entry.Value.Name);

        var announcements = await StudentAnnouncements.VisibleAsync(announcementRepository, enrolledClassGroupIds, today, now, cancellationToken);
        var isGoingByAnnouncement = (await announcementRepository.ListRepliesAsync(
                [.. announcements.Select(announcement => announcement.Id)], cancellationToken))
            .Where(reply => reply.ClientId == access.ClientId)
            .ToDictionary(reply => reply.AnnouncementId, reply => reply.IsGoing);
        items.AddRange(announcements
            .Select(announcement => Item(
                StudentAppNewsKind.Announcement, announcement.Id.ToString(CultureInfo.InvariantCulture), announcement.PublishedAt, announcement.Title, announcement.Body) with
            {
                AnnouncementId = announcement.Id,
                Category = announcement.Category,
                IsPinned = announcement.IsPinned,
                Action = announcement.Action,
                ActionUrl = announcement.ActionUrl,
                IsGoing = isGoingByAnnouncement.TryGetValue(announcement.Id, out var isGoing) ? isGoing : null,
                ImageUrl = announcement.ImagesInOrder.Count == 0 ? null : documentStorage.PublicUrlOf(announcement.ImagesInOrder[0]).AbsoluteUri,
                AudienceClassNames =
                [
                    .. announcement.ClassGroupIds
                        .Where(enrolledClassGroupNames.ContainsKey)
                        .Select(classGroupId => enrolledClassGroupNames[classGroupId]),
                ],
            }));

        items.AddRange((await orderRepository.ListAsync(family.Orders(awaitingPickupOnly: true, OrderLimit), null, cancellationToken))
            .Where(order => order.ReadyAt is not null)
            .Select(order => Item(StudentAppNewsKind.OrderReady, order.Id.ToString(CultureInfo.InvariantCulture), order.ReadyAt!.Value, orderId: order.Id)));

        items.AddRange((await feedbackRepository.ListUpdatedSinceAsync(family.PersonalDataStudentIds, now.AddDays(-FeedbackDays), cancellationToken))
            .Select(feedback => Item(
                StudentAppNewsKind.InstructorFeedback,
                string.Join(IdSeparator, feedback.StudentId, feedback.Date.ToString(IsoDateFormat, CultureInfo.InvariantCulture)),
                feedback.UpdatedAt,
                body: feedback.Text,
                studentFullNames: [studentNames.GetValueOrDefault(feedback.StudentId) ?? string.Empty],
                className: feedback.ClassGroupName,
                instructorFullName: instructorNames.GetValueOrDefault(feedback.InstructorId),
                classDate: feedback.Date)));

        items.AddRange(await UpcomingClassChangesAsync(
            students,
            currentEnrollments.ToLookup(enrollment => enrollment.StudentId),
            enrolledClassGroups,
            today,
            (session, classGroup, studentFullNames) => session.IsCancelled
                ? Item(
                    StudentAppNewsKind.ClassCancelled,
                    session.Id.ToString(CultureInfo.InvariantCulture),
                    session.CancelledAt ?? session.CreatedAt,
                    body: session.CancellationReason,
                    studentFullNames: studentFullNames,
                    className: classGroup.Name,
                    classDate: session.Date)
                : Item(
                    StudentAppNewsKind.ClassChanged,
                    session.Id.ToString(CultureInfo.InvariantCulture),
                    session.ChangedAt ?? session.CreatedAt,
                    studentFullNames: studentFullNames,
                    className: classGroup.Name,
                    instructorFullName: session.SubstituteInstructorId is { } substituteId ? instructorNames.GetValueOrDefault(substituteId) : null,
                    classDate: session.Date,
                    classStartTime: session.RescheduledStartTime?.ToString(ClassSchedule.TimeFormat, CultureInfo.InvariantCulture)),
            cancellationToken));

        var newestItems = items.OrderByDescending(item => item.IsPinned).ThenByDescending(item => item.OccurredAt).Take(ItemLimit).ToList();
        return new StudentAppNewsResponse(newestItems, newestItems.Count(item => item.IsUnread));
    }

    private async Task<IReadOnlyList<StudentAppNewsItemResponse>> UpcomingClassChangesAsync(
        IReadOnlyList<Domain.Students.Student> students,
        ILookup<long, Enrollment> currentEnrollments,
        IReadOnlyDictionary<long, ClassGroup> enrolledClassGroups,
        DateOnly today,
        Func<Domain.Sessions.ClassSession, ClassGroup, IReadOnlyList<string>, StudentAppNewsItemResponse> item,
        CancellationToken cancellationToken)
    {
        var changedSessions = (await sessionRepository.ListBetweenAsync(today, today.AddDays(ClassChangeLookAheadDays), cancellationToken))
            .Where(session => session.IsCancelled || session.IsChanged)
            .ToList();
        if (changedSessions.Count == 0)
        {
            return [];
        }

        var studentsBySession = new Dictionary<long, List<string>>();
        var classGroups = new Dictionary<long, ClassGroup>();
        foreach (var student in students)
        {
            foreach (var enrollment in currentEnrollments[student.Id])
            {
                var classGroup = enrolledClassGroups.GetValueOrDefault(enrollment.ClassGroupId);
                if (classGroup is null)
                {
                    continue;
                }

                foreach (var session in changedSessions.Where(session => session.ClassGroupId == enrollment.ClassGroupId
                    && session.Date >= enrollment.StartDate
                    && (enrollment.EndDate is not { } endDate || session.Date <= endDate)))
                {
                    classGroups[session.Id] = classGroup;
                    if (!studentsBySession.TryGetValue(session.Id, out var studentFullNames))
                    {
                        studentFullNames = [];
                        studentsBySession[session.Id] = studentFullNames;
                    }

                    studentFullNames.Add(student.User.FullName);
                }
            }
        }

        return
        [
            .. changedSessions
                .Where(session => studentsBySession.ContainsKey(session.Id))
                .Select(session => item(session, classGroups[session.Id], studentsBySession[session.Id])),
        ];
    }
}
