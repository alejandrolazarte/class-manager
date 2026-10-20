using ClassManager.Core.Audit;

namespace ClassManager.Core.UseCases.ImportExport;

public sealed record ExportFile(string FileName, string ContentType, byte[] Content, int RowCount) : ICountedRows;
