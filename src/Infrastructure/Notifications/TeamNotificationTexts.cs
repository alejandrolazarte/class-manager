using System.Globalization;

namespace ClassManager.Infrastructure.Notifications;

internal static class TeamNotificationTexts
{
    public const string OrdersUrl = "/today/orders";

    private const char NameSeparator = ' ';
    private const string TimeFormat = "HH:mm";

    private static readonly string[] Weekdays = ["dom", "lun", "mar", "mié", "jue", "vie", "sáb"];

    public static string SessionUrl(Guid classGroupId, DateOnly date) =>
        $"/today/{classGroupId}/{date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";

    public static string AbsenceTitle(string studentFullName) => $"{FirstNameOf(studentFullName)} avisó que no viene";

    public static string MakeupTitle(string studentFullName) => $"{FirstNameOf(studentFullName)} viene a recuperar";

    public static string ClassBody(string classGroupName, DateOnly date, TimeOnly startTime) =>
        $"{classGroupName} · {Weekdays[(int)date.DayOfWeek]} {date.Day}/{date.Month} {startTime.ToString(TimeFormat, CultureInfo.InvariantCulture)}";

    public static string OrderPlacedTitle(string clientFullName) => $"Nuevo pedido de {clientFullName}";

    private static string FirstNameOf(string fullName) =>
        fullName.Split(NameSeparator, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? fullName;
}
