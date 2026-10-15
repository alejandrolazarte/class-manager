using System.Globalization;
using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Infrastructure.Notifications;

internal sealed class SpanishNotificationTexts : INotificationTexts
{
    private const string TimeFormat = "HH:mm";

    private static readonly string[] Weekdays = ["domingo", "lunes", "martes", "miércoles", "jueves", "viernes", "sábado"];
    private static readonly string[] ShortWeekdays = ["dom", "lun", "mar", "mié", "jue", "vie", "sáb"];

    public string Language => AccountLanguage.Spanish;

    public string OrderReadyTitle => "Tu pedido está listo";

    public string GuardianConsentBody => "Entrá a la app para autorizar o no su acceso.";

    public string StudentAppInvitationDeclinedBody => "Si fue un error, podés volver a invitar desde su ficha.";

    public string GuardianConsentRefusedBody => "Su responsable no dio la autorización. Sus clases se siguen viendo desde la cuenta del responsable.";

    public string TeamInvitationDeclinedBody => "Si fue un error, podés volver a invitar desde Equipo.";

    public string AnnouncementReminderTitle(string announcementTitle) => $"Recordatorio: {announcementTitle}";

    public string GuardianConsentTitle(string studentFullName) => $"{PersonNames.FirstNameOf(studentFullName)} quiere usar la app";

    public string ClassCancelledTitle(string classGroupName) => $"Se suspende {classGroupName}";

    public string ClassCancelledBody(DateOnly date, TimeOnly startTime, string? reason)
    {
        var when = $"El {Weekdays[(int)date.DayOfWeek]} {date.Day}/{date.Month} a las {Time(startTime)} no hay clase.";
        return string.IsNullOrWhiteSpace(reason) ? when : $"{when} {reason}";
    }

    public string ClassChangedTitle(string classGroupName) => $"Cambio en {classGroupName}";

    public string ClassChangedBody(DateOnly date, TimeOnly startTime, string? substituteFullName)
    {
        var when = $"La clase del {Weekdays[(int)date.DayOfWeek]} {date.Day}/{date.Month} es a las {Time(startTime)}";
        return substituteFullName is null ? $"{when}." : $"{when} con {substituteFullName}.";
    }

    public string FeedbackTitle(string? instructorFullName, string studentFullName)
    {
        var studentFirstName = PersonNames.FirstNameOf(studentFullName);
        return instructorFullName is null
            ? $"Comentario sobre la clase de {studentFirstName}"
            : $"{instructorFullName} comentó la clase de {studentFirstName}";
    }

    public string AbsenceTitle(string studentFullName) => $"{PersonNames.FirstNameOf(studentFullName)} avisó que no viene";

    public string MakeupTitle(string studentFullName) => $"{PersonNames.FirstNameOf(studentFullName)} viene a recuperar";

    public string PackClassTitle(string studentFullName) => $"{PersonNames.FirstNameOf(studentFullName)} reservó una clase del pack";

    public string ClassBody(string classGroupName, DateOnly date, TimeOnly startTime) =>
        $"{classGroupName} · {ShortWeekdays[(int)date.DayOfWeek]} {date.Day}/{date.Month} {Time(startTime)}";

    public string StudentAppInvitationDeclinedTitle(string fullName) => $"{PersonNames.FirstNameOf(fullName)} rechazó la invitación a la app";

    public string GuardianConsentRefusedTitle(string fullName) => $"No autorizaron a {PersonNames.FirstNameOf(fullName)} a usar la app";

    public string TeamInvitationDeclinedTitle(string email) => $"{email} rechazó la invitación al equipo";

    public string OrderPlacedTitle(string orderNumber, string clientFullName) => $"Nuevo pedido {orderNumber} de {clientFullName}";

    private static string Time(TimeOnly time) => time.ToString(TimeFormat, CultureInfo.InvariantCulture);
}
