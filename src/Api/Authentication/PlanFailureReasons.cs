using Microsoft.AspNetCore.Authorization;

namespace ClassManager.Api.Authentication;

internal sealed class FeatureNotInPlanReason(IAuthorizationHandler handler, string featureCode)
    : AuthorizationFailureReason(handler, $"The plan doesn't include '{featureCode}'.")
{
    public string FeatureCode { get; } = featureCode;
}

internal sealed class SubscriptionInactiveReason(IAuthorizationHandler handler)
    : AuthorizationFailureReason(handler, "The subscription has ended.");
