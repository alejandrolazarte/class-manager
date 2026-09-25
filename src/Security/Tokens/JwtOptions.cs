using System.ComponentModel.DataAnnotations;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace ClassManager.Security.Tokens;

public sealed class JwtOptions : IValidatableObject
{
    public const string SectionName = "Authentication:Jwt";
    public const string DefaultTenantClaimType = "tenant_id";
    public const int MinimumSigningKeyByteCount = 32;

    private const string SigningKeyTooShortMessage = "Authentication:Jwt:SigningKey must be at least 32 bytes long.";

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Required]
    public string SigningKey { get; set; } = string.Empty;

    [Required]
    public string TenantClaimType { get; set; } = DefaultTenantClaimType;

    [Range(typeof(TimeSpan), "00:01:00", "01:00:00", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public TimeSpan AccessTokenLifetime { get; set; } = TimeSpan.FromMinutes(15);

    [Range(typeof(TimeSpan), "1.00:00:00", "90.00:00:00", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(30);

    public SymmetricSecurityKey CreateSigningKey() => new(Encoding.UTF8.GetBytes(SigningKey));

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Encoding.UTF8.GetByteCount(SigningKey) < MinimumSigningKeyByteCount)
        {
            yield return new ValidationResult(SigningKeyTooShortMessage, [nameof(SigningKey)]);
        }
    }
}
