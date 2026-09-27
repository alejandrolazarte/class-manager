namespace ClassManager.ImportExport.Parsing;

public sealed record ParsedImport(ColumnMapping Mapping, IReadOnlyList<ImportRow> Rows);
