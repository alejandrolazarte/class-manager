namespace ClassManager.Core.UseCases.ImportExport;

public sealed record ExportFile(string FileName, string ContentType, byte[] Content);
