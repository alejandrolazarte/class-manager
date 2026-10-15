namespace ClassManager.Infrastructure.Notifications;

internal static class PersonNames
{
    private const char NameSeparator = ' ';

    public static string FirstNameOf(string fullName) =>
        fullName.Split(NameSeparator, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? fullName;
}
