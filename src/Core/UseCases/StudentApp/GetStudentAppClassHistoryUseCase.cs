using System.Globalization;
using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.PrivateLessons;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record GetStudentAppClassHistoryQuery(long StudentId, string? Month) : IQuery;

public enum PastClassStatus
{
    Attended,
    Absent,
    AbsenceNotified,
    Cancelled,
    Unmarked,
}

public sealed record StudentAppPastClassResponse(
    string Name,
    DateOnly Date,
    string StartTime,
    string EndTime,
    string? InstructorFullName,
    string? Location,
    bool IsPrivateLesson,
    long? ClassGroupId,
    PastClassStatus Status,
    bool IsMakeup,
    bool IsPackBooking,
    StudentAppPaidWithPackResponse? PaidWithPack);

public sealed record StudentAppPaidWithPackResponse(string Name, DateOnly PurchasedOn)
{
    public static StudentAppPaidWithPackResponse? From(ClassPackPurchase? purchase) =>
        purchase is null ? null : new(purchase.Name, purchase.PurchasedOn);
}

public sealed record StudentAppClassHistoryResponse(string Month, IReadOnlyList<StudentAppPastClassResponse> Classes);

public sealed class GetStudentAppClassHistoryUseCase(
    IStudentAppAccess studentAppAccess,
    IStudentAppFamilyService studentAppFamily,
    IEnrollmentRepository enrollmentRepository,
    IClassGroupRepository classGroupRepository,
    IClassSessionRepository sessionRepository,
    IAttendanceRepository attendanceRepository,
    IAbsenceNoticeRepository absenceNoticeRepository,
    IMakeupBookingRepository makeupBookingRepository,
    IPackBookingRepository packBookingRepository,
    IPrivateLessonRepository privateLessonRepository,
    IInstructorRepository instructorRepository,
    IClassBalanceService classBalanceService,
    IBusinessCalendarService businessCalendar)
    : IUseCase<GetStudentAppClassHistoryQuery, StudentAppClassHistoryResponse>
{
    public async Task<Result<StudentAppClassHistoryResponse>> ExecuteAsync(GetStudentAppClassHistoryQuery command, CancellationToken cancellationToken)
    {
        var studentError = await AccountStudentRules.FindOwnStudentErrorAsync(studentAppFamily, command.StudentId, cancellationToken);
        if (studentError is not null)
        {
            return studentError;
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var month = command.Month is null
            ? BillingMonth.From(today)
            : BillingMonth.Parse(command.Month, nameof(GetStudentAppClassHistoryQuery.Month));
        if (month.IsFailure)
        {
            return month.Error!;
        }

        var firstDate = month.Value!.FirstDay;
        var yesterday = today.AddDays(-1);
        var lastDate = month.Value.LastDay < yesterday ? month.Value.LastDay : yesterday;
        if (lastDate < firstDate)
        {
            return new StudentAppClassHistoryResponse(month.Value.ToString(), []);
        }

        var studentIds = new[] { command.StudentId };
        var enrollments = await enrollmentRepository.ListByStudentInPeriodAsync(command.StudentId, firstDate, lastDate, cancellationToken);
        var makeups = await makeupBookingRepository.ListByStudentsBetweenAsync(studentIds, firstDate, lastDate, cancellationToken);
        var packBookings = await packBookingRepository.ListByStudentsBetweenAsync(studentIds, firstDate, lastDate, cancellationToken);
        IReadOnlyCollection<long> classGroupIds =
        [
            .. enrollments.Select(enrollment => enrollment.ClassGroupId)
                .Concat(makeups.Select(booking => booking.ClassGroupId))
                .Concat(packBookings.Select(booking => booking.ClassGroupId))
                .Distinct(),
        ];
        var classGroups = (await classGroupRepository.ListByIdsAsync(classGroupIds, cancellationToken))
            .Where(classGroup => classGroup.IsActive)
            .ToDictionary(classGroup => classGroup.Id);
        var sessions = (await sessionRepository.ListBetweenAsync(firstDate, lastDate, cancellationToken))
            .ToDictionary(session => (session.ClassGroupId, session.Date));
        var attendances = (await attendanceRepository.ListByStudentBetweenAsync(command.StudentId, firstDate, lastDate, cancellationToken))
            .ToDictionary(attendance => (attendance.ClassGroupId, attendance.Date), attendance => attendance.Status);
        var notices = (await absenceNoticeRepository.ListByStudentsBetweenAsync(studentIds, firstDate, lastDate, cancellationToken))
            .Select(notice => (notice.ClassGroupId, notice.Date))
            .ToHashSet();
        var instructorNames = (await instructorRepository.ListAllAsync(cancellationToken))
            .ToDictionary(instructor => instructor.Id, instructor => instructor.User.FullName);
        var access = await studentAppAccess.GetAsync(cancellationToken);
        var balance = (await classBalanceService.CalculateAsync([access!.ClientId], cancellationToken))[access.ClientId];
        var packPurchases = balance.PaidClasses
            .Where(paidClass => paidClass.AttendedClass.StudentId == command.StudentId
                && paidClass.AttendedClass.Date >= firstDate
                && paidClass.AttendedClass.Date <= lastDate)
            .GroupBy(paidClass => (paidClass.AttendedClass.ClassGroupId, paidClass.AttendedClass.Date))
            .ToDictionary(paidClasses => paidClasses.Key, paidClasses => new Queue<ClassPackPurchase>(paidClasses.Select(paidClass => paidClass.Purchase)));
        ClassPackPurchase? PackPurchaseOf(long? classGroupId, DateOnly date) =>
            packPurchases.TryGetValue((classGroupId, date), out var purchases) && purchases.Count > 0 ? purchases.Dequeue() : null;

        var pastClasses = new List<StudentAppPastClassResponse>();
        var listedClasses = new HashSet<(long ClassGroupId, DateOnly Date)>();
        void AddGroupClass(long classGroupId, DateOnly date, bool isMakeup, bool isPackBooking)
        {
            if (!classGroups.TryGetValue(classGroupId, out var classGroup) || !listedClasses.Add((classGroupId, date)))
            {
                return;
            }

            var session = sessions.GetValueOrDefault((classGroupId, date));
            var status = StatusOf(
                session?.IsCancelled ?? false,
                attendances.TryGetValue((classGroupId, date), out var attendance) ? attendance : null,
                notices.Contains((classGroupId, date)));
            pastClasses.Add(GroupClass(classGroup, session, date, instructorNames, status, isMakeup, isPackBooking, PackPurchaseOf(classGroupId, date)));
        }

        foreach (var enrollment in enrollments)
        {
            var enrollmentFirstDate = enrollment.StartDate > firstDate ? enrollment.StartDate : firstDate;
            var enrollmentLastDate = enrollment.EndDate is { } endDate && endDate < lastDate ? endDate : lastDate;
            for (var date = enrollmentFirstDate; date <= enrollmentLastDate; date = date.AddDays(1))
            {
                if (classGroups.TryGetValue(enrollment.ClassGroupId, out var classGroup) && classGroup.Schedule.MeetsOn(date.DayOfWeek))
                {
                    AddGroupClass(enrollment.ClassGroupId, date, isMakeup: false, isPackBooking: false);
                }
            }
        }

        foreach (var booking in makeups)
        {
            AddGroupClass(booking.ClassGroupId, booking.Date, isMakeup: true, isPackBooking: false);
        }

        foreach (var booking in packBookings)
        {
            AddGroupClass(booking.ClassGroupId, booking.Date, isMakeup: false, isPackBooking: true);
        }

        var privateLessons = await privateLessonRepository.ListBetweenAsync(firstDate, lastDate, cancellationToken);
        foreach (var lesson in privateLessons)
        {
            var lessonStudent = lesson.Students.FirstOrDefault(lessonStudent => lessonStudent.StudentId == command.StudentId);
            if (lessonStudent is not null)
            {
                pastClasses.Add(PrivateLesson(lesson, lessonStudent, instructorNames, PackPurchaseOf(null, lesson.Date)));
            }
        }

        return new StudentAppClassHistoryResponse(
            month.Value.ToString(),
            [
                .. pastClasses
                    .OrderBy(pastClass => pastClass.Date)
                    .ThenBy(pastClass => pastClass.StartTime, StringComparer.Ordinal),
            ]);
    }

    private static PastClassStatus StatusOf(bool isCancelled, AttendanceStatus? attendance, bool absenceNotified)
    {
        if (isCancelled)
        {
            return PastClassStatus.Cancelled;
        }

        if (attendance == AttendanceStatus.Present)
        {
            return PastClassStatus.Attended;
        }

        if (absenceNotified)
        {
            return PastClassStatus.AbsenceNotified;
        }

        return attendance == AttendanceStatus.Absent ? PastClassStatus.Absent : PastClassStatus.Unmarked;
    }

    private static StudentAppPastClassResponse GroupClass(
        ClassGroup classGroup,
        ClassSession? session,
        DateOnly date,
        IReadOnlyDictionary<long, string> instructorNames,
        PastClassStatus status,
        bool isMakeup,
        bool isPackBooking,
        ClassPackPurchase? packPurchase)
    {
        var startTime = session?.EffectiveStartTime(classGroup.StartTime) ?? classGroup.StartTime;
        var instructorId = session?.EffectiveInstructorId(classGroup.InstructorId) ?? classGroup.InstructorId;
        return new StudentAppPastClassResponse(
            classGroup.Name,
            date,
            Format(startTime),
            Format(startTime.AddMinutes(classGroup.DurationMinutes)),
            instructorNames.GetValueOrDefault(instructorId),
            classGroup.Location,
            IsPrivateLesson: false,
            classGroup.Id,
            status,
            isMakeup,
            isPackBooking,
            StudentAppPaidWithPackResponse.From(packPurchase));
    }

    private static StudentAppPastClassResponse PrivateLesson(
        PrivateLesson lesson,
        PrivateLessonStudent lessonStudent,
        IReadOnlyDictionary<long, string> instructorNames,
        ClassPackPurchase? packPurchase) =>
        new(
            lesson.IsTrial ? GetStudentAppHomeUseCase.TrialName : GetStudentAppHomeUseCase.PrivateLessonName,
            lesson.Date,
            Format(lesson.StartTime),
            Format(lesson.EndTime),
            instructorNames.GetValueOrDefault(lesson.InstructorId),
            lesson.Location,
            IsPrivateLesson: true,
            ClassGroupId: null,
            StatusOf(lesson.IsCancelled, lessonStudent.Status, absenceNotified: false),
            IsMakeup: false,
            IsPackBooking: false,
            StudentAppPaidWithPackResponse.From(packPurchase));

    private static string Format(TimeOnly time) => time.ToString(ClassSchedule.TimeFormat, CultureInfo.InvariantCulture);
}
