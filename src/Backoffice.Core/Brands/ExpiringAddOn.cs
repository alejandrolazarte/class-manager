namespace ClassManager.Backoffice.Core.Brands;

public sealed record ExpiringAddOn(long BrandId, string BrandName, string FeatureCode, DateOnly ExpiredOn);
