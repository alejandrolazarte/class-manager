using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Domain.Users;

namespace ClassManager.Core.Services;

public sealed class UserPhotoUrlService(IUserPhotoRepository photoRepository, IDocumentStorageService documentStorage)
    : IUserPhotoUrlService
{
    public async Task<IReadOnlyDictionary<long, string>> OfUsersAsync(IEnumerable<long> userIds, CancellationToken cancellationToken)
    {
        var distinctUserIds = userIds.Distinct().ToList();
        return distinctUserIds.Count == 0
            ? new Dictionary<long, string>()
            : UrlsOf(await photoRepository.ListVisibleOfUsersAsync(distinctUserIds, cancellationToken));
    }

    public async Task<IReadOnlyDictionary<long, string>> OfStudentsAsync(IEnumerable<long> studentIds, CancellationToken cancellationToken)
    {
        var distinctStudentIds = studentIds.Distinct().ToList();
        return distinctStudentIds.Count == 0
            ? new Dictionary<long, string>()
            : UrlsOf(await photoRepository.ListVisibleOfStudentsAsync(distinctStudentIds, cancellationToken));
    }

    public async Task<IReadOnlyDictionary<long, string>> OfInstructorsAsync(IEnumerable<long?> instructorIds, CancellationToken cancellationToken)
    {
        var knownInstructorIds = instructorIds.OfType<long>().Distinct().ToList();
        return knownInstructorIds.Count == 0
            ? new Dictionary<long, string>()
            : UrlsOf(await photoRepository.ListVisibleOfInstructorsAsync(knownInstructorIds, cancellationToken));
    }

    private Dictionary<long, string> UrlsOf(IReadOnlyList<OwnedPhoto> photos) =>
        photos.ToDictionary(photo => photo.OwnerId, photo => documentStorage.PrivateUrlOf(photo.Document).ToString());
}
