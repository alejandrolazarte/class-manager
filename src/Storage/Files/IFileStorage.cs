namespace ClassManager.Storage.Files;

public interface IFileStorage
{
    Task SaveAsync(FileToStore file, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(string path, FileVisibility visibility, CancellationToken cancellationToken);

    Uri PublicUrlOf(string path);
}
