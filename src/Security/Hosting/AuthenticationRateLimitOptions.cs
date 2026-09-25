using System.ComponentModel.DataAnnotations;

namespace ClassManager.Security.Hosting;

public sealed class AuthenticationRateLimitOptions
{
    public const string SectionName = "Authentication:RateLimit";

    [Range(1, int.MaxValue)]
    public int PermitLimit { get; set; } = 10;

    [Range(typeof(TimeSpan), "00:00:01", "01:00:00", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);
}
