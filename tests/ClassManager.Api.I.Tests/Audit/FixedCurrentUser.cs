using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Audit;

internal sealed record FixedCurrentUser(Guid? AccountId, long? UserId) : ICurrentUser;
