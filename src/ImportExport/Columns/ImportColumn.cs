namespace ClassManager.ImportExport.Columns;

public sealed record ImportColumn(string Key, string Header, ColumnType Type, bool IsRequired = false)
{
    public IReadOnlyList<string> Aliases { get; init; } = [];

    public string? Example { get; init; }
}
