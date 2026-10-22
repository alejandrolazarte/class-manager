using System.Globalization;

namespace ClassManager.Infrastructure.Notifications;

internal static class TeamNotificationLinks
{
    public const string OrdersUrl = "/today/orders";
    public const string TeamUrl = "/settings/team";

    private const string DateFormat = "yyyy-MM-dd";

    public static string SessionUrl(long classGroupId, DateOnly date) =>
        $"/today/{classGroupId}/{date.ToString(DateFormat, CultureInfo.InvariantCulture)}";

    public static string ClientUrl(long clientId) => $"/students/clients/{clientId}";
}
