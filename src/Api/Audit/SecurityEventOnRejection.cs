using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.Audit;

internal sealed record SecurityEventOnRejection(SecurityEventType Event);
