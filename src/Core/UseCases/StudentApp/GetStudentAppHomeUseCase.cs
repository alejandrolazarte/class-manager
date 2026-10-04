using System.Globalization;
using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Achievements;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record GetStudentAppHomeQuery;

public sealed class GetStudentAppHomeUseCase(
    IStudentAppAccess studentAppAccess,
    IBusinessRepository businessRepository,
    IClientRepository clientRepository,
    IStudentRepository studentRepository,
    IEnrollmentRepository enrollmentRepository,
    IClassGroupRepository classGroupRepository,
    IClassSessionRepository sessionRepository,
    IAttendanceRepository attendanceRepository,
    IClassFeedbackRepository feedbackRepository,
    IAbsenceNoticeRepository absenceNoticeRepository,
    IMakeupBookingRepository makeupBookingRepository,
    IPackBookingRepository packBookingRepository,
    IAchievementLevelRepository levelRepository,
    IPrivateLessonRepository privateLessonRepository,
    IInstructorRepository instructorRepository,
    IFeeScheduleRepository feeScheduleRepository,
    IPaymentRepository paymentRepository,
    IClassBalanceService classBalanceService,
    IBusinessCalendarService businessCalendar)
    : IUseCase<GetStudentAppHomeQuery, StudentAppHomeResponse>
{
    public const int LookAheadDays = 14;
    public const int NextClassLimit = 5;

    private const string NoAccessMessage = "This account is not linked to a client.";
    private const string NoAccessCode = "student.no_access";

    public async Task<Result<StudentAppHomeResponse>> ExecuteAsync(GetStudentAppHomeQuery command, CancellationToken cancellationToken)
    {
        var access = await studentAppAccess.GetAsync(cancellationToken);
        var client = access is null ? null : await clientRepository.GetByIdAsync(access.ClientId, cancellationToken);
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (access is null || client is null || business is null)
        {
            return Result.Unauthorized<StudentAppHomeResponse>(NoAccessMessage, NoAccessCode);
        }

        var today = await businessCalendar.TodayAsync(cancellationToken);
        var lastDate = today.AddDays(LookAheadDays);
        var students = await studentRepository.ListByClientAsync(client.Id, cancellationToken);
        var instructorNames = (await instructorRepository.ListAllAsync(cancellationToken))
            .ToDictionary(instructor => instructor.Id, instructor => instructor.FullName);
        var sessions = (await sessionRepository.ListBetweenAsync(today, lastDate, cancellationToken))
            .ToDictionary(session => (session.ClassGroupId, session.Date));
        var privateLessons = (await privateLessonRepository.ListBetweenAsync(today, lastDate, cancellationToken))
            .Where(lesson => lesson.Students.Any(lessonStudent => students.Any(student => student.Id == lessonStudent.StudentId)))
            .ToList();

        var levels = await levelRepository.ListAsync(cancellationToken);
        var studentIds = students.Select(student => student.Id).ToList();
        var attendanceMarks = (await attendanceRepository.ListMarksByStudentsAsync(
                studentIds, AttendanceStreaks.FirstDayToLoad(today), today, cancellationToken))
            .ToLookup(studentMark => studentMark.StudentId, studentMark => studentMark.Mark);
        var attendedClasses = await attendanceRepository.CountAttendedClassesByStudentsAsync(studentIds, cancellationToken);
        var notifiedAbsences = (await absenceNoticeRepository.ListByStudentsBetweenAsync(studentIds, today, lastDate, cancellationToken))
            .ToHashSet();
        var bookedMakeups = (await makeupBookingRepository.ListByStudentsBetweenAsync(studentIds, today, lastDate, cancellationToken))
            .ToLookup(booking => booking.StudentId);
        var bookedPackClasses = (await packBookingRepository.ListByStudentsBetweenAsync(studentIds, today, lastDate, cancellationToken))
            .ToLookup(booking => booking.StudentId);
        var latestFeedbacks = (await feedbackRepository.ListLatestByStudentsAsync(studentIds, cancellationToken))
            .ToDictionary(feedback => feedback.StudentId);

        var studentResponses = new List<AccountStudentResponse>();
        foreach (var student in students.OrderBy(student => student.FullName, StringComparer.CurrentCultureIgnoreCase))
        {
            var enrollments = await enrollmentRepository.ListCurrentByStudentAsync(student.Id, today, cancellationToken);
            var nextClasses = new List<StudentAppNextClassResponse>();
            foreach (var enrollment in enrollments)
            {
                var classGroup = await classGroupRepository.GetByIdAsync(enrollment.ClassGroupId, cancellationToken);
                if (classGroup is not { IsActive: true })
                {
                    continue;
                }

                for (var date = today; date <= lastDate; date = date.AddDays(1))
                {
                    if (date < enrollment.StartDate || (enrollment.EndDate is { } endDate && date > endDate) || !classGroup.Schedule.MeetsOn(date.DayOfWeek))
                    {
                        continue;
                    }

                    nextClasses.Add(GroupClass(
                        classGroup,
                        sessions.GetValueOrDefault((classGroup.Id, date)),
                        date,
                        instructorNames,
                        notifiedAbsences.Contains(new NotifiedAbsence(student.Id, classGroup.Id, date)),
                        isMakeup: false));
                }
            }

            foreach (var booking in bookedMakeups[student.Id])
            {
                var classGroup = await classGroupRepository.GetByIdAsync(booking.ClassGroupId, cancellationToken);
                if (classGroup is not null)
                {
                    nextClasses.Add(GroupClass(
                        classGroup,
                        sessions.GetValueOrDefault((classGroup.Id, booking.Date)),
                        booking.Date,
                        instructorNames,
                        absenceNotified: false,
                        isMakeup: true));
                }
            }

            foreach (var booking in bookedPackClasses[student.Id])
            {
                var classGroup = await classGroupRepository.GetByIdAsync(booking.ClassGroupId, cancellationToken);
                if (classGroup is not null)
                {
                    nextClasses.Add(GroupClass(
                        classGroup,
                        sessions.GetValueOrDefault((classGroup.Id, booking.Date)),
                        booking.Date,
                        instructorNames,
                        absenceNotified: false,
                        isMakeup: false) with
                    { IsPackBooking = true });
                }
            }

            nextClasses.AddRange(privateLessons
                .Where(lesson => lesson.Students.Any(lessonStudent => lessonStudent.StudentId == student.Id))
                .Select(lesson => new StudentAppNextClassResponse(
                    lesson.IsTrial ? TrialName : PrivateLessonName,
                    lesson.Date,
                    Format(lesson.StartTime),
                    Format(lesson.EndTime),
                    instructorNames.GetValueOrDefault(lesson.InstructorId),
                    lesson.Location,
                    IsPrivateLesson: true,
                    lesson.IsCancelled,
                    ClassGroupId: null,
                    AbsenceNotified: false,
                    IsMakeup: false)));

            studentResponses.Add(new AccountStudentResponse(
                student.Id,
                student.FullName,
                [
                    .. nextClasses
                        .OrderBy(nextClass => nextClass.Date)
                        .ThenBy(nextClass => nextClass.StartTime, StringComparer.Ordinal)
                        .Take(NextClassLimit),
                ],
                Attendance(attendanceMarks[student.Id].ToList(), attendedClasses.GetValueOrDefault(student.Id), levels, today),
                latestFeedbacks.TryGetValue(student.Id, out var feedback)
                    ? new StudentAppFeedbackResponse(feedback.Date, feedback.ClassGroupName, instructorNames.GetValueOrDefault(feedback.InstructorId), feedback.Text)
                    : null));
        }

        return new StudentAppHomeResponse(
            business.Name,
            business.CurrencyCode,
            client.FullName,
            studentResponses,
            await BillingAsync(client.Id, today, cancellationToken),
            levels);
    }

    private const string PrivateLessonName = "Clase particular";
    private const string TrialName = "Clase de prueba";

    private static StudentAppNextClassResponse GroupClass(
        ClassGroup classGroup,
        Domain.Sessions.ClassSession? session,
        DateOnly date,
        IReadOnlyDictionary<Guid, string> instructorNames,
        bool absenceNotified,
        bool isMakeup)
    {
        var startTime = session?.EffectiveStartTime(classGroup.StartTime) ?? classGroup.StartTime;
        var instructorId = session?.EffectiveInstructorId(classGroup.InstructorId) ?? classGroup.InstructorId;
        return new StudentAppNextClassResponse(
            classGroup.Name,
            date,
            Format(startTime),
            Format(startTime.AddMinutes(classGroup.DurationMinutes)),
            instructorNames.GetValueOrDefault(instructorId),
            classGroup.Location,
            IsPrivateLesson: false,
            session?.IsCancelled ?? false,
            classGroup.Id,
            absenceNotified,
            isMakeup);
    }

    private static StudentAppAttendanceResponse Attendance(
        IReadOnlyCollection<AttendanceMark> marks,
        int attendedClasses,
        IReadOnlyList<LevelDefinition> levels,
        DateOnly today)
    {
        var streak = AttendanceStreaks.Calculate(marks, today);
        var level = LevelLadder.LevelFor(levels, attendedClasses);
        return new StudentAppAttendanceResponse(
            streak.Weeks,
            streak.Since,
            attendedClasses,
            streak.RecentWeeks,
            streak.BestWeeks,
            level,
            Medals.Earned(new MedalProgress(attendedClasses, level, streak.BestWeeks), marks, today));
    }

    private async Task<StudentAppBillingResponse> BillingAsync(Guid clientId, DateOnly today, CancellationToken cancellationToken)
    {
        var month = BillingMonth.From(today);
        var plan = FeeTimeline.PlanIn(await feeScheduleRepository.ListClientPlanChangesAsync([clientId], cancellationToken), month);
        if (plan.PaysPerClass)
        {
            var balance = (await classBalanceService.CalculateAsync([clientId], cancellationToken))[clientId];
            return new StudentAppBillingResponse(
                plan.Kind, MonthlyFee: null, new StudentAppClassBalanceResponse(balance.AvailableClasses, balance.UnpaidClasses));
        }

        var defaultFee = FeeTimeline.DefaultFeeIn(await feeScheduleRepository.ListDefaultFeeChangesAsync(cancellationToken), month);
        var fee = plan.MonthlyFee(defaultFee);
        var paid = (await paymentRepository.SumByClientForMonthAsync(month.FirstDay, cancellationToken)).GetValueOrDefault(clientId);
        return new StudentAppBillingResponse(
            plan.Kind,
            new StudentAppMonthlyFeeResponse(month.ToString(), fee, paid, fee is null ? 0 : Math.Max(fee.Value - paid, 0), FeeRules.StatusOf(fee, paid)),
            Classes: null);
    }

    private static string Format(TimeOnly time) => time.ToString(ClassSchedule.TimeFormat, CultureInfo.InvariantCulture);
}
