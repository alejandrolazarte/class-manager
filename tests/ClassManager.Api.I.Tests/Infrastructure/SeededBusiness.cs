using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed record SeededBusiness(Business Business, HttpClient HttpClient, Guid OwnerUserId);
