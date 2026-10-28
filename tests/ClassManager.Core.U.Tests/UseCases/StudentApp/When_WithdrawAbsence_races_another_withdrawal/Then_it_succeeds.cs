using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Core.U.Tests.UseCases.StudentApp.When_WithdrawAbsence_races_another_withdrawal;

public sealed class Then_it_succeeds
{
    [Fact]
    public async Task Then_it_succeeds_Run()
    {
        var classDate = TestData.Today.AddDays(7);
        var client = TestData.Client();
        var student = Student.Create(client, TestData.StudentFullName, null, null, TestData.Today, TestData.Now).Value!.WithId();
        var classGroup = ClassGroup.Create("Clase", TestIds.Next(), ClassSchedule.Create([classDate.DayOfWeek], "18:00", 45).Value!, 8, null).Value!.WithId();
        var session = ClassSession.Create(classGroup.Id, classDate, TestData.Now).WithId();
        var studentAppAccess = new StudentAppAccess(Guid.NewGuid(), TestIds.Next(), TestIds.Next(), client.Id);
        var studentAppFamily = new Mock<IStudentAppFamilyService>();
        studentAppFamily.Setup(family => family.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StudentAppFamily(studentAppAccess, TestData.Business(), TestData.Today, client, [new StudentAppFamilyMember(student, FamilyMemberView.Everything)], [], CanBuy: true));
        var classGroups = new Mock<IClassGroupRepository>();
        classGroups.Setup(repository => repository.GetByIdAsync(classGroup.Id, It.IsAny<CancellationToken>())).ReturnsAsync(classGroup);
        var enrollments = new Mock<IEnrollmentRepository>();
        enrollments.Setup(repository => repository.ListRosterOnAsync(classGroup.Id, classDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new RosterEntry(TestIds.Next(), student.Id, TestData.StudentFullName, null, client.Id, TestData.ClientFullName, TestData.Today, null)]);
        var sessions = new Mock<IClassSessionRepository>();
        sessions.Setup(repository => repository.FindForUpdateAsync(classGroup.Id, classDate, It.IsAny<CancellationToken>())).ReturnsAsync(session);
        sessions.Setup(repository => repository.ListBetweenAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        var notices = new Mock<IAbsenceNoticeRepository>();
        notices.Setup(repository => repository.FindForUpdateAsync(session.Id, student.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AbsenceNotice.Create(session, student.Id, keepsStreak: false, TestData.Now));
        notices.Setup(repository => repository.ListByStudentsBetweenAsync(
                It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        var bookings = new Mock<IMakeupBookingRepository>();
        bookings.Setup(repository => repository.ListByStudentsBetweenAsync(
                It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        var calendar = new Mock<IBusinessCalendarService>();
        calendar.Setup(service => service.TodayAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Today);
        calendar.Setup(service => service.LocalNowAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Now.UtcDateTime);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(work => work.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new ConcurrencyConflictException());
        var useCase = new WithdrawAbsenceUseCase(
            studentAppFamily.Object,
            classGroups.Object,
            new MakeupRepositories(notices.Object, bookings.Object, new Mock<IPackBookingRepository>().Object, sessions.Object, enrollments.Object),
            unitOfWork.Object,
            calendar.Object);

        var response = await useCase.ExecuteAsync(new WithdrawAbsenceCommand(student.Id, classGroup.Id, classDate), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
    }
}
