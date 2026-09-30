using System.Globalization;

namespace ClassManager.Infrastructure.Notifications;

internal static class FamilyPushTexts
{
    public const string NewsUrl = "/family/news";
    public const string OrdersUrl = "/family/orders";
    public const string OrderReadyTitle = "Tu pedido está listo";

    private const int BodyMaxLength = 160;
    private const string Ellipsis = "…";
    private const char NameSeparator = ' ';

    private static readonly string[] Weekdays = ["domingo", "lunes", "martes", "miércoles", "jueves", "viernes", "sábado"];

    public static string ClassCancelledTitle(string classGroupName) => $"Se suspende {classGroupName}";

    public static string ClassCancelledBody(DateOnly date, TimeOnly startTime, string? reason)
    {
        var when = $"El {Weekdays[(int)date.DayOfWeek]} {date.Day}/{date.Month} a las {startTime.ToString("HH:mm", CultureInfo.InvariantCulture)} no hay clase.";
        return string.IsNullOrWhiteSpace(reason) ? when : $"{when} {reason}";
    }

    public static string FeedbackTitle(string? instructorFullName, string studentFullName)
    {
        var studentFirstName = studentFullName.Split(NameSeparator, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? studentFullName;
        return instructorFullName is null
            ? $"Comentario sobre la clase de {studentFirstName}"
            : $"{instructorFullName} comentó la clase de {studentFirstName}";
    }

    public static string Shorten(string text) =>
        text.Length <= BodyMaxLength ? text : $"{text[..(BodyMaxLength - Ellipsis.Length)]}{Ellipsis}";
}
