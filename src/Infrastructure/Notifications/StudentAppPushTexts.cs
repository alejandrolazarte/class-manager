using System.Globalization;

namespace ClassManager.Infrastructure.Notifications;

internal static class StudentAppPushTexts
{
    public const string NewsUrl = "/student-app/news";
    public const string OrdersUrl = "/student-app/orders";
    public const string OrderReadyTitle = "Tu pedido está listo";
    public const string HomeUrl = "/student-app";
    public const string GuardianConsentBody = "Entrá a la app para autorizar o no su acceso.";

    private const int BodyMaxLength = 160;
    private const string Ellipsis = "…";
    private const char NameSeparator = ' ';
    private const string TimeFormat = "HH:mm";

    private static readonly string[] Weekdays = ["domingo", "lunes", "martes", "miércoles", "jueves", "viernes", "sábado"];

    public static string GuardianConsentTitle(string studentFullName) => $"{FirstNameOf(studentFullName)} quiere usar la app";

    public static string ClassCancelledTitle(string classGroupName) => $"Se suspende {classGroupName}";

    public static string ClassCancelledBody(DateOnly date, TimeOnly startTime, string? reason)
    {
        var when = $"El {Weekdays[(int)date.DayOfWeek]} {date.Day}/{date.Month} a las {startTime.ToString(TimeFormat, CultureInfo.InvariantCulture)} no hay clase.";
        return string.IsNullOrWhiteSpace(reason) ? when : $"{when} {reason}";
    }

    public static string ClassChangedTitle(string classGroupName) => $"Cambio en {classGroupName}";

    public static string ClassChangedBody(DateOnly date, TimeOnly startTime, string? substituteFullName)
    {
        var when = $"La clase del {Weekdays[(int)date.DayOfWeek]} {date.Day}/{date.Month} es a las {startTime.ToString(TimeFormat, CultureInfo.InvariantCulture)}";
        return substituteFullName is null ? $"{when}." : $"{when} con {substituteFullName}.";
    }

    public static string FeedbackTitle(string? instructorFullName, string studentFullName)
    {
        var studentFirstName = FirstNameOf(studentFullName);
        return instructorFullName is null
            ? $"Comentario sobre la clase de {studentFirstName}"
            : $"{instructorFullName} comentó la clase de {studentFirstName}";
    }

    public static string Shorten(string text) =>
        text.Length <= BodyMaxLength ? text : $"{text[..(BodyMaxLength - Ellipsis.Length)]}{Ellipsis}";

    private static string FirstNameOf(string fullName) =>
        fullName.Split(NameSeparator, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? fullName;
}
