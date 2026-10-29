namespace ClassManager.Core.Abstractions.Storage;

public interface IUserPhotoUrlService
{
    Task<IReadOnlyDictionary<long, string>> OfStudentsAsync(IEnumerable<long> studentIds, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, string>> OfInstructorsAsync(IEnumerable<long?> instructorIds, CancellationToken cancellationToken);
}
