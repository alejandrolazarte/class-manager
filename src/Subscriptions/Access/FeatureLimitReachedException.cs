namespace ClassManager.Subscriptions.Access;

public sealed class FeatureLimitReachedException : Exception
{
    public FeatureLimitReachedException()
        : this(string.Empty)
    {
    }

    public FeatureLimitReachedException(string featureCode)
        : base(FeatureErrorCodes.LimitReachedMessage)
    {
        FeatureCode = featureCode;
    }

    public FeatureLimitReachedException(string message, Exception innerException)
        : base(message, innerException)
    {
        FeatureCode = string.Empty;
    }

    public string FeatureCode { get; }
}
