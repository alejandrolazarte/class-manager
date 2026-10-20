namespace ClassManager.Backoffice.Core.Brands;

public sealed record ExpiringAddOn(Guid BrandId, string BrandName, string FeatureCode, DateOnly ExpiredOn);
