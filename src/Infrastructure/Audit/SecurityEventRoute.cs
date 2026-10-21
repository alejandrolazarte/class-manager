namespace ClassManager.Infrastructure.Audit;

public sealed record SecurityEventRoute(string Method, string Route, int Count);
