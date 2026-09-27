using ClassManager.ImportExport.Tabular;
using ClassManager.ImportExport.Tabular.Csv;

namespace ClassManager.ImportExport.Xlsx;

public sealed class XlsxOrCsvTabularReader(XlsxTabularReader xlsxReader, CsvTabularReader csvReader) : ITabularReader
{
    private static readonly byte[] ZipLocalFileSignature = [0x50, 0x4B, 0x03, 0x04];

    public TabularReadResult Read(ReadOnlyMemory<byte> content) =>
        content.Span.StartsWith(ZipLocalFileSignature) ? xlsxReader.Read(content) : csvReader.Read(content);
}
