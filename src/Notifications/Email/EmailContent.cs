using System.Text;

namespace ClassManager.Notifications.Email;

public sealed record EmailContent(string Eyebrow, string Title, string Intro, string FooterNote)
{
    public const string TotalLabel = "Total";

    private const string ParagraphBreak = "\n\n";
    private const char LineBreak = '\n';
    private const char ActionLabelEnd = ':';
    private const string ListItemPrefix = "- ";
    private const string LabelSeparator = ": ";

    public IReadOnlyList<EmailLine> Lines { get; init; } = [];

    public string? Total { get; init; }

    public EmailNote? Note { get; init; }

    public EmailAction? Action { get; init; }

    public bool ListsItems => Lines.Count > 0;

    public string ToPlainText()
    {
        var text = new StringBuilder()
            .Append(Title).Append(ParagraphBreak)
            .Append(Intro).Append(ParagraphBreak);
        if (ListsItems)
        {
            foreach (var line in Lines)
            {
                text.Append(ListItemPrefix).Append(line.Description).Append(LabelSeparator).Append(line.Amount).Append(LineBreak);
            }

            if (Total is not null)
            {
                text.Append(TotalLabel).Append(LabelSeparator).Append(Total).Append(LineBreak);
            }

            text.Append(LineBreak);
        }

        if (Note is not null)
        {
            text.Append(Note.Label is null ? Note.Text : Note.Label + LabelSeparator + Note.Text).Append(ParagraphBreak);
        }

        if (Action is not null)
        {
            text.Append(Action.Label).Append(ActionLabelEnd).Append(LineBreak).Append(Action.Url).Append(ParagraphBreak);
        }

        return text.Append(FooterNote).ToString();
    }
}

public sealed record EmailLine(string Description, string Amount);

public sealed record EmailNote(string Text, string? Label = null);

public sealed record EmailAction(string Label, string Url, bool ShowsLinkFallback = false);
