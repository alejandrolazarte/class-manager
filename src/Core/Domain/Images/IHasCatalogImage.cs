namespace ClassManager.Core.Domain.Images;

public interface IHasCatalogImage
{
    Guid Id { get; }

    string? ImageUrl { get; }

    void ChangeImage(Uri imageUrl);

    void RemoveImage();
}
