namespace ClassManager.Infrastructure.Email;

public sealed class SmtpOptions
{
    public const string SectionName = "Email:Smtp";
    public const int DefaultPort = 587;

    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = DefaultPort;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;

    public bool IsConfigured => !string.IsNullOrWhiteSpace(Host);
}
