using ClassManager.Core.Domain.Documents;

namespace ClassManager.Core.Domain.Users;

public sealed record OwnedPhoto(long OwnerId, Document Document);
