using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.Services;

public sealed class StudentAppFamilyService(
    IStudentAppAccess studentAppAccess,
    IClientRepository clientRepository,
    IStudentRepository studentRepository,
    IBusinessRepository businessRepository,
    IIdentityService identityService,
    TimeProvider timeProvider)
    : IStudentAppFamilyService
{
    private StudentAppFamily? _resolvedFamily;
    private bool _isResolved;

    public async Task<StudentAppFamily?> GetAsync(CancellationToken cancellationToken)
    {
        if (!_isResolved)
        {
            _resolvedFamily = await ResolveAsync(cancellationToken);
            _isResolved = true;
        }

        return _resolvedFamily;
    }

    private async Task<StudentAppFamily?> ResolveAsync(CancellationToken cancellationToken)
    {
        var access = await studentAppAccess.GetAsync(cancellationToken);
        var client = access is null ? null : await clientRepository.GetByIdAsync(access.ClientId, cancellationToken);
        var business = await businessRepository.GetCurrentAsync(cancellationToken);
        if (access is null || client is null || business is null || (!access.PaysForFamily && access.UserId is null))
        {
            return null;
        }

        var today = business.TodayAt(timeProvider.GetUtcNow());
        var students = await studentRepository.ListByClientAsync(client.Id, cancellationToken);
        var accountBirthDates = (await identityService.ListAccountsAsync(
                [.. students.Select(student => student.User.AccountId).OfType<Guid>().Distinct()], cancellationToken))
            .ToDictionary(account => account.UserId, account => account.BirthDate);
        DateOnly? BirthDateOf(Student student) => FamilyVisibility.YoungestBirthDate(
        [
            student.User.BirthDate,
            student.User.AccountId is { } accountId ? accountBirthDates.GetValueOrDefault(accountId) : null,
        ]);
        bool IsOwn(Student student) => access.PaysForFamily ? student.UserId == client.UserId : student.Id == access.StudentId;

        var views = students.Select(student => new StudentAppFamilyMember(
                student,
                FamilyVisibility.ViewOf(IsOwn(student), access.PaysForFamily, BirthDateOf(student), today, business.Country.OwnAccountMinimumAge)))
            .ToList();
        var ownStudent = students.FirstOrDefault(IsOwn);
        return new StudentAppFamily(
            access,
            business,
            today,
            client,
            [.. views.Where(member => member.View != FamilyMemberView.None)],
            [.. views.Where(member => member.View == FamilyMemberView.None).Select(member => member.Student.UserId).Distinct()],
            FamilyVisibility.CanBuy(access.PaysForFamily, ownStudent is null ? null : BirthDateOf(ownStudent), today),
            students);
    }
}
