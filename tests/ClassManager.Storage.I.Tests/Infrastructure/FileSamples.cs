namespace ClassManager.Storage.I.Tests.Infrastructure;

internal static class FileSamples
{
    public const string PngContentType = "image/png";

    public static readonly byte[] Png =
        Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==");

    public static FileToStore PngAt(string path) => new(path, Png, PngContentType);
}
