namespace ClassManager.Storage.Files;

public static class FilePath
{
    public const char Separator = '/';

    private const string InvalidSegmentMessage = "A file path segment cannot be empty, contain '/' or '\\', or be '.' or '..'.";
    private const string CurrentFolder = ".";
    private const string ParentFolder = "..";
    private const char BackSeparator = '\\';

    public static string Combine(params string[] segments)
    {
        if (segments.Length == 0 || segments.Any(IsInvalidSegment))
        {
            throw new ArgumentException(InvalidSegmentMessage, nameof(segments));
        }

        return string.Join(Separator, segments);
    }

    private static bool IsInvalidSegment(string segment) =>
        string.IsNullOrWhiteSpace(segment)
        || segment.Contains(Separator, StringComparison.Ordinal)
        || segment.Contains(BackSeparator, StringComparison.Ordinal)
        || segment is CurrentFolder or ParentFolder;
}
