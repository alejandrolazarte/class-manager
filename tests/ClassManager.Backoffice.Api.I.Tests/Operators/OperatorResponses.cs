namespace ClassManager.Backoffice.Api.I.Tests.Operators;

internal sealed record OperatorListResponse(IReadOnlyList<OperatorResponse> Operators, IReadOnlyList<AccessRequestResponse> AccessRequests);

internal sealed record OperatorResponse(Guid LoginId, string Email, string? Role);

internal sealed record AccessRequestResponse(Guid Id, string Email, string Code);
