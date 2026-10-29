using ClassManager.Tenancy.AspNetCore.Persistence;

namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class UserPhotoRepository(AppDbContext context) : IUserPhotoRepository
{
    public Task<UserPhoto?> FindByUserAsync(long userId, CancellationToken cancellationToken) =>
        context.UserPhotos.FirstOrDefaultAsync(photo => photo.UserId == userId, cancellationToken);

    public Task<UserPhoto?> FindForUpdateAsync(long photoId, CancellationToken cancellationToken) =>
        context.UserPhotos.FirstOrDefaultAsync(photo => photo.Id == photoId, cancellationToken);

    public async Task<IReadOnlyList<UserPhoto>> ListAwaitingGuardianAsync(IReadOnlyCollection<long> userIds, CancellationToken cancellationToken) =>
        await context.UserPhotos
            .AsNoTracking()
            .Where(photo => photo.AwaitsGuardian && userIds.Contains(photo.UserId))
            .OrderBy(photo => photo.Id)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<OwnedPhoto>> ListVisibleOfUsersAsync(IReadOnlyCollection<long> userIds, CancellationToken cancellationToken) =>
        await VisiblePhotos()
            .AsNoTracking()
            .Where(photo => userIds.Contains(photo.UserId))
            .Select(photo => new OwnedPhoto(photo.UserId, photo.Document))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<OwnedPhoto>> ListVisibleOfStudentsAsync(IReadOnlyCollection<long> studentIds, CancellationToken cancellationToken) =>
        await context.Students
            .AsNoTracking()
            .Where(student => studentIds.Contains(student.Id))
            .Join(VisiblePhotos(), student => student.UserId, photo => photo.UserId, (student, photo) => new OwnedPhoto(student.Id, photo.Document))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<OwnedPhoto>> ListVisibleOfInstructorsAsync(IReadOnlyCollection<long> instructorIds, CancellationToken cancellationToken) =>
        await context.Instructors
            .AsNoTracking()
            .Where(instructor => instructorIds.Contains(instructor.Id))
            .Join(VisiblePhotos(), instructor => instructor.UserId, photo => photo.UserId, (instructor, photo) => new OwnedPhoto(instructor.Id, photo.Document))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Document>> DeleteEveryPhotoOfAccountAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var accountUserIds = context.Users
            .IgnoreQueryFilters([TenantModelBuilderExtensions.TenantQueryFilter, SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .Where(user => user.AccountId == accountId)
            .Select(user => user.Id);
        var photos = context.UserPhotos.IgnoreTenantFilter().Where(photo => accountUserIds.Contains(photo.UserId));
        var documentIds = await photos.Select(photo => photo.DocumentId).ToListAsync(cancellationToken);
        var documents = await context.Documents.IgnoreTenantFilter().AsNoTracking()
            .Where(document => documentIds.Contains(document.Id))
            .ToListAsync(cancellationToken);
        await photos.ExecuteDeleteAsync(cancellationToken);
        await context.Documents.IgnoreTenantFilter().Where(document => documentIds.Contains(document.Id)).ExecuteDeleteAsync(cancellationToken);
        return documents;
    }

    private IQueryable<UserPhoto> VisiblePhotos() => context.UserPhotos.Where(photo => !photo.AwaitsGuardian);

    public void Add(UserPhoto photo) => context.UserPhotos.Add(photo);

    public void Remove(UserPhoto photo) => context.UserPhotos.Remove(photo);
}
