using System.Globalization;

namespace ClassManager.Infrastructure.Notifications;

internal static class TeamNotificationTexts
{
    public const string OrdersUrl = "/today/orders";
    public const string TeamUrl = "/settings/team";
    public const string StudentAppInvitationDeclinedBody = "Si fue un error, podés volver a invitar desde su ficha.";
    public const string TeamInvitationDeclinedBody = "Si fue un error, podés volver a invitar desde Equipo.";

    private const char NameSeparator = ' ';
    private const string TimeFormat = "HH:mm";

    private static readonly string[] Weekdays = ["dom", "lun", "mar", "mié", "jue", "vie", "sáb"];

    public static string SessionUrl(Guid classGroupId, DateOnly date) =>
        $"/today/{classGroupId}/{date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";

    public static string AbsenceTitle(string studentFullName) => $"{FirstNameOf(studentFullName)} avisó que no viene";

    public static string MakeupTitle(string studentFullName) => $"{FirstNameOf(studentFullName)} viene a recuperar";

    public static string PackClassTitle(string studentFullName) => $"{FirstNameOf(studentFullName)} reservó una clase del pack";

    public static string ClassBody(string classGroupName, DateOnly date, TimeOnly startTime) =>
        $"{classGroupName} · {Weekdays[(int)date.DayOfWeek]} {date.Day}/{date.Month} {startTime.ToString(TimeFormat, CultureInfo.InvariantCulture)}";

    public static string ClientUrl(Guid clientId) => $"/students/clients/{clientId}";

    public static string StudentAppInvitationDeclinedTitle(string fullName) => $"{FirstNameOf(fullName)} rechazó la invitación a la app";

    public static string TeamInvitationDeclinedTitle(string email) => $"{email} rechazó la invitación al equipo";

    public static string OrderPlacedTitle(string orderNumber, string clientFullName) => $"Nuevo pedido {orderNumber} de {clientFullName}";

    private static string FirstNameOf(string fullName) =>
        fullName.Split(NameSeparator, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? fullName;
}
