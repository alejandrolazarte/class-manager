using ClassManager.Core.Abstractions.Storage;

namespace ClassManager.Core.U.Tests;

internal sealed class NoUserPhotoUrls : IUserPhotoUrlService
{
    private static readonly IReadOnlyDictionary<long, string> NoPhotos = new Dictionary<long, string>();

    public Task<IReadOnlyDictionary<long, string>> OfUsersAsync(IEnumerable<long> userIds, CancellationToken cancellationToken) =>
        Task.FromResult(NoPhotos);

    public Task<IReadOnlyDictionary<long, string>> OfStudentsAsync(IEnumerable<long> studentIds, CancellationToken cancellationToken) =>
        Task.FromResult(NoPhotos);

    public Task<IReadOnlyDictionary<long, string>> OfInstructorsAsync(IEnumerable<long?> instructorIds, CancellationToken cancellationToken) =>
        Task.FromResult(NoPhotos);
}
