using ClassManager.Core.Abstractions.Security;
using ClassManager.Infrastructure.Persistence;

namespace ClassManager.Infrastructure.Notifications;

internal sealed record StudentAppRecipientAccounts(IReadOnlyCollection<long> ClientIds, IReadOnlyCollection<Guid> AccountIds);

internal sealed class StudentAppRecipients(AppDbContext context, IIdentityService identityService, TimeProvider timeProvider)
{
    public async Task<StudentAppRecipientAccounts> SeeingAsync(
        IReadOnlyCollection<long> studentIds,
        FamilyMemberView minimumView,
        CancellationToken cancellationToken)
    {
        var students = await context.Students.AsNoTracking()
            .Where(student => studentIds.Contains(student.Id))
            .Select(student => new
            {
                student.Id,
                student.ClientId,
                student.UserId,
                ClientUserId = student.Client!.UserId,
                student.User.BirthDate,
                student.User.AccountId,
            })
            .ToListAsync(cancellationToken);
        var business = await context.Businesses.AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.Id == context.CurrentTenantId, cancellationToken);
        if (students.Count == 0 || business is null)
        {
            return new StudentAppRecipientAccounts([], []);
        }

        var clientIds = students.Select(student => student.ClientId).Distinct().ToList();
        var accounts = await context.ClientAccounts.AsNoTracking()
            .Where(account => clientIds.Contains(account.ClientId) && account.User.AccountId != null)
            .Select(account => new { account.ClientId, account.StudentId, AccountId = account.User.AccountId!.Value })
            .ToListAsync(cancellationToken);
        var accountBirthDates = (await identityService.ListAccountsAsync(
                [.. students.Select(student => student.AccountId).OfType<Guid>().Distinct()], cancellationToken))
            .ToDictionary(account => account.UserId, account => account.BirthDate);
        var today = business.TodayAt(timeProvider.GetUtcNow());

        var recipients = accounts
            .Where(account => students
                .Where(student => student.ClientId == account.ClientId)
                .Any(student => FamilyVisibility.ViewOf(
                    account.StudentId is null ? student.UserId == student.ClientUserId : account.StudentId == student.Id,
                    account.StudentId is null,
                    FamilyVisibility.YoungestBirthDate(
                    [
                        student.BirthDate,
                        student.AccountId is { } accountId ? accountBirthDates.GetValueOrDefault(accountId) : null,
                    ]),
                    today,
                    business.Country.OwnAccountMinimumAge) >= minimumView))
            .ToList();
        return new StudentAppRecipientAccounts(
            [.. recipients.Select(account => account.ClientId).Distinct()],
            [.. recipients.Select(account => account.AccountId).Distinct()]);
    }
}
