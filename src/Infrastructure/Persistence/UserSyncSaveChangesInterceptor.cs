using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Clients;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Infrastructure.Persistence;

internal sealed class UserSyncSaveChangesInterceptor(TimeProvider timeProvider, IServiceProvider serviceProvider) : SaveChangesInterceptor
{
    private const string UserIdProperty = "UserId";

    private static readonly EntityState[] SavedStates = [EntityState.Added, EntityState.Modified];

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SyncUsersAsync(eventData.Context, CancellationToken.None).GetAwaiter().GetResult();
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        await SyncUsersAsync(eventData.Context, cancellationToken);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task SyncUsersAsync(DbContext? dbContext, CancellationToken cancellationToken)
    {
        if (dbContext is not AppDbContext context)
        {
            return;
        }

        context.ChangeTracker.DetectChanges();
        foreach (var entry in SavedEntries<Client>(context))
        {
            await SyncClientAsync(context, entry, cancellationToken);
        }

        foreach (var entry in SavedEntries<Student>(context))
        {
            await SyncStudentAsync(context, entry, cancellationToken);
        }

        foreach (var entry in SavedEntries<Instructor>(context))
        {
            await SyncInstructorAsync(context, entry, cancellationToken);
        }

        foreach (var entry in AddedEntries<BusinessMember>(context))
        {
            await LinkMemberAsync(context, entry, cancellationToken);
        }

        foreach (var entry in AddedEntries<ClientAccount>(context))
        {
            await LinkClientAccountAsync(context, entry, cancellationToken);
        }
    }

    private async Task SyncClientAsync(AppDbContext context, EntityEntry<Client> entry, CancellationToken cancellationToken)
    {
        var client = entry.Entity;
        var user = await FindUserAsync(context, client.UserId, cancellationToken) ?? AddUser(context, client.TenantId, client.FullName);
        client.LinkUser(user);
        user.Rename(client.FullName);
        user.ChangeEmail(client.Email);
        user.ChangePhoneNumber(client.PhoneNumber);
    }

    private async Task SyncStudentAsync(AppDbContext context, EntityEntry<Student> entry, CancellationToken cancellationToken)
    {
        var student = entry.Entity;
        var client = await FindClientAsync(context, student.ClientId, cancellationToken);
        var attendingContactUser = client is not null && AttendingContact.IsTheContact(client.FullName, student.FullName)
            ? await FindUserAsync(context, client.UserId, cancellationToken)
            : null;
        var user = await FindUserAsync(context, student.UserId, cancellationToken)
            ?? attendingContactUser
            ?? AddUser(context, student.TenantId, student.FullName);
        student.LinkUser(user);
        if (user != attendingContactUser)
        {
            user.Rename(student.FullName);
            user.ChangeEmail(student.Email);
        }

        user.ChangeBirthDate(student.BirthDate);
    }

    private async Task SyncInstructorAsync(AppDbContext context, EntityEntry<Instructor> entry, CancellationToken cancellationToken)
    {
        var instructor = entry.Entity;
        var user = await FindUserAsync(context, instructor.UserId, cancellationToken) ?? AddUser(context, instructor.TenantId, instructor.FullName);
        instructor.LinkUser(user);
        user.Rename(instructor.FullName);
        user.ChangeEmail(instructor.Email);
    }

    private async Task LinkMemberAsync(AppDbContext context, EntityEntry<BusinessMember> entry, CancellationToken cancellationToken)
    {
        var member = entry.Entity;
        if (member.UserId is not null)
        {
            return;
        }

        var instructor = member.InstructorId is { } instructorId ? await FindInstructorAsync(context, instructorId, cancellationToken) : null;
        var user = await FindUserAsync(context, instructor?.UserId, cancellationToken)
            ?? await FindUserByAccountAsync(context, member.TenantId, member.AccountId, cancellationToken);
        if (user is null)
        {
            var identityService = serviceProvider.GetRequiredService<IIdentityService>();
            var account = (await identityService.ListAccountsAsync([member.AccountId], cancellationToken)).SingleOrDefault();
            if (account is null)
            {
                return;
            }

            user = AddUser(context, member.TenantId, account.FullName);
            user.ChangeEmail(account.Email);
        }

        SetUserId(entry, user);
        await LinkAccountWhenFreeAsync(context, user, member.AccountId, cancellationToken);
    }

    private static async Task LinkClientAccountAsync(AppDbContext context, EntityEntry<ClientAccount> entry, CancellationToken cancellationToken)
    {
        var clientAccount = entry.Entity;
        if (clientAccount.UserId is not null)
        {
            return;
        }

        var personUserId = clientAccount.StudentId is { } studentId
            ? (await FindStudentAsync(context, studentId, cancellationToken))?.UserId
            : (await FindClientAsync(context, clientAccount.ClientId, cancellationToken))?.UserId;
        var user = await FindUserAsync(context, personUserId, cancellationToken);
        if (user is null)
        {
            return;
        }

        SetUserId(entry, user);
        await LinkAccountWhenFreeAsync(context, user, clientAccount.AccountId, cancellationToken);
    }

    private static async Task LinkAccountWhenFreeAsync(AppDbContext context, User user, Guid accountId, CancellationToken cancellationToken)
    {
        if (user.AccountId is not null || await FindUserByAccountAsync(context, user.TenantId, accountId, cancellationToken) is not null)
        {
            return;
        }

        user.LinkAccount(accountId);
    }

    private User AddUser(AppDbContext context, Guid tenantId, string fullName)
    {
        var user = User.Create(tenantId, fullName, timeProvider.GetUtcNow());
        context.Users.Add(user);
        return user;
    }

    private static void SetUserId<TEntity>(EntityEntry<TEntity> entry, User user)
        where TEntity : class =>
        entry.Property(UserIdProperty).CurrentValue = user.Id;

    private static List<EntityEntry<TEntity>> SavedEntries<TEntity>(AppDbContext context)
        where TEntity : class =>
        context.ChangeTracker.Entries<TEntity>().Where(entry => SavedStates.Contains(entry.State)).ToList();

    private static List<EntityEntry<TEntity>> AddedEntries<TEntity>(AppDbContext context)
        where TEntity : class =>
        context.ChangeTracker.Entries<TEntity>().Where(entry => entry.State == EntityState.Added).ToList();

    private static async Task<User?> FindUserAsync(AppDbContext context, Guid? userId, CancellationToken cancellationToken) =>
        userId is not { } id
            ? null
            : context.Users.Local.FirstOrDefault(user => user.Id == id)
                ?? await context.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

    private static async Task<User?> FindUserByAccountAsync(AppDbContext context, Guid tenantId, Guid accountId, CancellationToken cancellationToken) =>
        context.Users.Local.FirstOrDefault(user => user.AccountId == accountId && (user.TenantId == tenantId || user.TenantId == Guid.Empty))
            ?? await context.Users.FirstOrDefaultAsync(user => user.AccountId == accountId, cancellationToken);

    private static async Task<Client?> FindClientAsync(AppDbContext context, Guid clientId, CancellationToken cancellationToken) =>
        context.Clients.Local.FirstOrDefault(client => client.Id == clientId)
            ?? await context.Clients.FirstOrDefaultAsync(client => client.Id == clientId, cancellationToken);

    private static async Task<Student?> FindStudentAsync(AppDbContext context, Guid studentId, CancellationToken cancellationToken) =>
        context.Students.Local.FirstOrDefault(student => student.Id == studentId)
            ?? await context.Students.FirstOrDefaultAsync(student => student.Id == studentId, cancellationToken);

    private static async Task<Instructor?> FindInstructorAsync(AppDbContext context, Guid instructorId, CancellationToken cancellationToken) =>
        context.Instructors.Local.FirstOrDefault(instructor => instructor.Id == instructorId)
            ?? await context.Instructors.FirstOrDefaultAsync(instructor => instructor.Id == instructorId, cancellationToken);
}
