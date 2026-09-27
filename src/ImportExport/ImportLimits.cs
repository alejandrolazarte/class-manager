namespace ClassManager.ImportExport;

public sealed record ImportLimits(int MaximumFileSizeInBytes, int MaximumRowCount)
{
    public const int DefaultMaximumFileSizeInBytes = 1024 * 1024;
    public const int DefaultMaximumRowCount = 1000;

    public static readonly ImportLimits Default = new(DefaultMaximumFileSizeInBytes, DefaultMaximumRowCount);
}
