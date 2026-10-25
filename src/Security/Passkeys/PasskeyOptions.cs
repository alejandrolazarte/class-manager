using System.ComponentModel.DataAnnotations;

namespace ClassManager.Security.Passkeys;

public sealed class PasskeyOptions
{
    public const string SectionName = "Passkeys";

    [Required]
    public string ServerDomain { get; set; } = string.Empty;

    [MinLength(1)]
    public string[] AllowedOrigins { get; set; } = [];
}
