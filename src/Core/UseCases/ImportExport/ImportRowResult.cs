using ClassManager.ImportExport.Parsing;

namespace ClassManager.Core.UseCases.ImportExport;

public sealed record ImportRowResult(int Line, ImportRowStatus Status, IReadOnlyList<ImportCellError> Errors)
{
    public static ImportRowResult Valid(int line) => new(line, ImportRowStatus.Valid, []);

    public static ImportRowResult Error(int line, IReadOnlyList<ImportCellError> errors) => new(line, ImportRowStatus.Error, errors);

    public static ImportRowResult Error(int line, ImportCellError error) => new(line, ImportRowStatus.Error, [error]);

    public static ImportRowResult Skipped(int line, ImportCellError reason) => new(line, ImportRowStatus.Skipped, [reason]);
}
