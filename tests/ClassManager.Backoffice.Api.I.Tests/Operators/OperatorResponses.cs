namespace ClassManager.Backoffice.Api.I.Tests.Operators;

internal sealed record OperatorListResponse(IReadOnlyList<OperatorResponse> Operators, IReadOnlyList<AccessRequestResponse> AccessRequests);

internal sealed record OperatorResponse(long LoginId, string Email, string? Role);

internal sealed record AccessRequestResponse(long Id, string Email, string Code);
