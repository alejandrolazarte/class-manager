namespace ClassManager.Core.Abstractions.OnlinePayments;

public sealed record OnboardingLinkRequest(string ExternalAccountId, Uri RefreshUrl, Uri ReturnUrl);
