namespace ClassManager.Infrastructure.Audit;

public enum SecurityEventType
{
    Forbidden,
    SignInFailed,
    RefreshTokenRejected,
    UnauthorizedBurst,
    PlanLimit,
}
