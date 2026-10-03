using ClassManager.Core.Common;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Core.UseCases.Subscriptions;

public static class FeatureErrors
{
    public static ResultError LimitReached(string featureCode) =>
        new(FeatureErrorCodes.LimitReached, FeatureErrorCodes.LimitReachedMessage, ErrorKind.Forbidden)
        {
            Details = new Dictionary<string, object?> { [FeatureErrorCodes.FeatureDetail] = featureCode },
        };
}
