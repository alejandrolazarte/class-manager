using System.ComponentModel.DataAnnotations;

namespace ClassManager.Backoffice.Infrastructure;

public sealed class BackofficeOptions
{
    public const string SectionName = "Backoffice";

    public long? PlatformOrganizationId { get; set; }

    [Required]
    public string WebAppUrl { get; set; } = string.Empty;

    [Required]
    public string StripeDashboardUrl { get; set; } = "https://dashboard.stripe.com/";

    public GoogleSignInOptions Google { get; set; } = new();
}

public sealed class GoogleSignInOptions
{
    public string Authority { get; set; } = "https://accounts.google.com";

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;
}
