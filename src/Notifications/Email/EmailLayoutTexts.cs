namespace ClassManager.Notifications.Email;

public sealed record EmailLayoutTexts(string LanguageCode, string TotalLabel, string LinkFallback)
{
    public static EmailLayoutTexts Spanish { get; } = new("es", "Total", "Si el botón no funciona, copiá este link en el navegador:");
}
