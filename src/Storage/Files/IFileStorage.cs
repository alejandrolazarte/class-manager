namespace ClassManager.Storage.Files;

public interface IFileStorage
{
    Task<Uri> SaveAsync(FileToStore file, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Uri fileUrl, CancellationToken cancellationToken);
}
