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

    [Range(typeof(TimeSpan), "01:00:00", "90.00:00:00", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(30);

    /// <summary>
    /// <see langword="true"/> (the default) gives every rotated refresh token a full <see cref="RefreshTokenLifetime"/>,
    /// so an active session never ends; <see langword="false"/> keeps the expiry of the session's first token, so a
    /// session ends <see cref="RefreshTokenLifetime"/> after it started however often it is refreshed.
    /// </summary>
    public bool ExtendSessionOnRefresh { get; set; } = true;

    public SymmetricSecurityKey CreateSigningKey() => new(Encoding.UTF8.GetBytes(SigningKey));

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Encoding.UTF8.GetByteCount(SigningKey) < MinimumSigningKeyByteCount)
        {
            yield return new ValidationResult(SigningKeyTooShortMessage, [nameof(SigningKey)]);
        }
    }
}
