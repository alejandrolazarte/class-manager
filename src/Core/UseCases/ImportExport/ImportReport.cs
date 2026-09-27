using ClassManager.ImportExport.Parsing;

namespace ClassManager.Core.UseCases.ImportExport;

public sealed record ImportReport(
    string Module,
    IReadOnlyList<MappedHeader> Columns,
    ImportSummary Summary,
    IReadOnlyList<ImportRowResult> Rows);
