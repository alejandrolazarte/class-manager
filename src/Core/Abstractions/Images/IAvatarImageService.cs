using ClassManager.Core.Domain.Documents;

namespace ClassManager.Core.Abstractions.Images;

public interface IAvatarImageService
{
    DocumentContent? ToAvatar(byte[] content, int sideInPixels);
}
