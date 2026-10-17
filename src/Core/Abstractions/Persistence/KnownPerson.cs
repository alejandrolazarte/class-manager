using ClassManager.Core.Domain.Users;

namespace ClassManager.Core.Abstractions.Persistence;

public sealed record KnownPerson(Guid UserId, string FullName, IReadOnlyList<PersonRole> Roles);
