using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ClassManager.Notifications.Email;

public static partial class BrandedEmailHtml
{
    public const string TemplateResourceName = "ClassManager.Notifications.Email.Templates.BrandedEmail.html";

    private const string PreheaderPlaceholder = "Preheader";
    private const string BrandBadgePlaceholder = "BrandBadge";
    private const string BrandNamePlaceholder = "BrandName";
    private const string SectionsPlaceholder = "Sections";
    private const string InitialsPlaceholder = "Initials";
    private const string LogoContentIdPlaceholder = "LogoContentId";
    private const string UrlPlaceholder = "Url";
    private const string LabelPlaceholder = "Label";
    private const string AmountPlaceholder = "Amount";
    private const int MaximumInitials = 2;
    private const char NameSeparator = ' ';

    private const string LogoBadge =
        """<td style="background:#ffffff;border-radius:10px;padding:4px"><img src="cid:{{LogoContentId}}" height="40" alt="{{BrandName}}" style="display:block;height:40px;width:auto;max-width:160px;border:0;border-radius:6px"></td>""";

    private const string InitialsBadge =
        """<td width="48" height="48" align="center" style="width:48px;height:48px;background:#ffffff;border-radius:10px;font-weight:bold;font-size:18px;color:{{Primary}}">{{Initials}}</td>""";

    private const string ButtonSection =
        """
        <tr><td style="padding:28px 32px 0">
        <table role="presentation" cellpadding="0" cellspacing="0" border="0"><tr><td bgcolor="{{Primary}}" style="border-radius:10px"><a href="{{Url}}" style="display:block;white-space:nowrap;padding:14px 26px;font-size:15px;font-weight:bold;color:#ffffff;text-decoration:none">{{Label}}</a></td></tr></table>
        </td></tr>
        """;

    private const string NoteSection =
        """
        <tr><td style="padding:20px 32px 0">
        <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="background:{{PrimarySoft}};border-radius:10px"><tr><td style="padding:14px 18px;font-size:14px;line-height:20px;color:#10212f">{{Text}}</td></tr></table>
        </td></tr>
        """;

    private const string LabeledNoteSection =
        """
        <tr><td style="padding:8px 32px 0">
        <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="background:{{PrimarySoft}};border-radius:10px"><tr><td style="padding:16px 18px">
        <p style="margin:0 0 4px;font-size:13px;font-weight:bold;color:{{NoteLabel}}">{{Label}}</p>
        <p style="margin:0;font-size:15px;line-height:22px;color:#10212f">{{Text}}</p>
        </td></tr></table>
        </td></tr>
        """;

    private const string LinkFallbackSection =
        """
        <tr><td style="padding:20px 32px 0;font-size:12px;line-height:18px;color:#657179">Si el botón no funciona, copiá este link en el navegador:<br><a href="{{Url}}" style="color:{{Primary}};word-break:break-all">{{Url}}</a></td></tr>
        """;

    private const string LinesStart =
        """
        <tr><td style="padding:24px 32px 0">
        <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="border-top:1px solid #dfe5ea">
        """;

    private const string LineRow =
        """
        <tr><td style="padding:14px 0;font-size:15px;border-bottom:1px solid #dfe5ea">{{Description}}</td><td align="right" style="padding:14px 0;font-size:15px;white-space:nowrap;border-bottom:1px solid #dfe5ea">{{Amount}}</td></tr>
        """;

    private const string TotalRow =
        """
        <tr><td style="padding:16px 0;font-size:16px;font-weight:bold">{{Label}}</td><td align="right" style="padding:16px 0;font-size:18px;font-weight:bold;white-space:nowrap;color:{{Primary}}">{{Amount}}</td></tr>
        """;

    private const string LinesEnd =
        """
        </table>
        </td></tr>
        """;

    private static readonly Lazy<string> Template = new(LoadTemplate);

    public static string Render(EmailContent content, EmailBrand brand)
    {
        var palette = EmailPalette.From(brand.ThemeColor, brand.AccentColor);
        var colors = new Dictionary<string, string>
        {
            [nameof(EmailPalette.Primary)] = palette.Primary,
            [nameof(EmailPalette.PrimaryStrong)] = palette.PrimaryStrong,
            [nameof(EmailPalette.PrimarySoft)] = palette.PrimarySoft,
            [nameof(EmailPalette.NoteLabel)] = palette.NoteLabel,
            [nameof(EmailPalette.Accent)] = palette.Accent,
        };

        var values = new Dictionary<string, string>(colors)
        {
            [PreheaderPlaceholder] = Encode(content.Intro),
            [BrandBadgePlaceholder] = BrandBadge(brand, colors),
            [BrandNamePlaceholder] = Encode(brand.DisplayName),
            [nameof(EmailContent.Eyebrow)] = Encode(content.Eyebrow),
            [nameof(EmailContent.Title)] = Encode(content.Title),
            [nameof(EmailContent.Intro)] = Encode(content.Intro),
            [SectionsPlaceholder] = Sections(content, colors),
            [nameof(EmailContent.FooterNote)] = Encode(content.FooterNote),
        };
        return Fill(Template.Value, values);
    }

    public static string InitialsOf(string displayName) =>
        string.Concat(displayName
            .Split(NameSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Take(MaximumInitials)
            .Select(word => char.ToUpperInvariant(word[0])));

    private static string BrandBadge(EmailBrand brand, Dictionary<string, string> colors) =>
        brand.Logo is null
            ? Fill(InitialsBadge, new Dictionary<string, string>(colors) { [InitialsPlaceholder] = Encode(InitialsOf(brand.DisplayName)) })
            : Fill(LogoBadge, new Dictionary<string, string>
            {
                [LogoContentIdPlaceholder] = Encode(brand.Logo.ContentId),
                [BrandNamePlaceholder] = Encode(brand.DisplayName),
            });

    private static string Sections(EmailContent content, Dictionary<string, string> colors)
    {
        var sections = new StringBuilder();
        if (content.ListsItems)
        {
            sections.Append(Lines(content, colors)).Append(Note(content.Note, colors)).Append(Button(content.Action, colors));
        }
        else
        {
            sections.Append(Button(content.Action, colors)).Append(Note(content.Note, colors));
            if (content.Action is { ShowsLinkFallback: true })
            {
                sections.Append(Fill(LinkFallbackSection, new Dictionary<string, string>(colors) { [UrlPlaceholder] = Encode(content.Action.Url) }));
            }
        }

        return sections.ToString();
    }

    private static string Lines(EmailContent content, Dictionary<string, string> colors)
    {
        var lines = new StringBuilder(LinesStart);
        foreach (var line in content.Lines)
        {
            lines.Append(Fill(LineRow, new Dictionary<string, string>
            {
                [nameof(EmailLine.Description)] = Encode(line.Description),
                [nameof(EmailLine.Amount)] = Encode(line.Amount),
            }));
        }

        if (content.Total is not null)
        {
            lines.Append(Fill(TotalRow, new Dictionary<string, string>(colors)
            {
                [LabelPlaceholder] = Encode(EmailContent.TotalLabel),
                [AmountPlaceholder] = Encode(content.Total),
            }));
        }

        return lines.Append(LinesEnd).ToString();
    }

    private static string Note(EmailNote? note, Dictionary<string, string> colors) =>
        note is null
            ? string.Empty
            : Fill(note.Label is null ? NoteSection : LabeledNoteSection, new Dictionary<string, string>(colors)
            {
                [nameof(EmailNote.Text)] = Encode(note.Text),
                [nameof(EmailNote.Label)] = Encode(note.Label ?? string.Empty),
            });

    private static string Button(EmailAction? action, Dictionary<string, string> colors) =>
        action is null
            ? string.Empty
            : Fill(ButtonSection, new Dictionary<string, string>(colors)
            {
                [nameof(EmailAction.Url)] = Encode(action.Url),
                [nameof(EmailAction.Label)] = Encode(action.Label),
            });

    private static string Fill(string template, Dictionary<string, string> values) =>
        PlaceholderPattern().Replace(template, match => values[match.Groups[1].Value]);

    private static string Encode(string text) => WebUtility.HtmlEncode(text);

    private static string LoadTemplate()
    {
        using var stream = typeof(BrandedEmailHtml).Assembly.GetManifestResourceStream(TemplateResourceName)
            ?? throw new InvalidOperationException(TemplateResourceName);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    [GeneratedRegex(@"\{\{(\w+)\}\}")]
    private static partial Regex PlaceholderPattern();
}
