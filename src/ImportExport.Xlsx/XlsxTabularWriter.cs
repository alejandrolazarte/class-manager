using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Xml;
using ClassManager.ImportExport.Tabular;

namespace ClassManager.ImportExport.Xlsx;

public sealed class XlsxTabularWriter : ITabularWriter
{
    public const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public const string XlsxFileExtension = ".xlsx";

    private const string WorksheetElement = "worksheet";
    private const string SheetDataElement = "sheetData";
    private const string FrozenHeaderRowXml =
        """<sheetViews><sheetView workbookViewId="0"><pane ySplit="1" topLeftCell="A2" activePane="bottomLeft" state="frozen"/></sheetView></sheetViews>""";
    private static readonly CompositeFormat TextColumnsXmlFormat = CompositeFormat.Parse("""<cols><col min="1" max="{0}" width="24" style="2" customWidth="1"/></cols>""");
    private const string HeaderStyleIndex = "1";
    private const string TextStyleIndex = "2";
    private const string XmlPrefix = "xml";
    private const string SpaceAttribute = "space";
    private const string PreserveSpace = "preserve";
    private const int FirstRowNumber = 1;

    private const string ContentTypesXml =
        """<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types"><Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/><Default Extension="xml" ContentType="application/xml"/><Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/><Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/><Override PartName="/xl/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"/></Types>""";

    private const string PackageRelationshipsXml =
        """<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/></Relationships>""";

    private const string WorkbookXml =
        """<?xml version="1.0" encoding="UTF-8" standalone="yes"?><workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"><sheets><sheet name="Sheet1" sheetId="1" r:id="rId1"/></sheets></workbook>""";

    private const string WorkbookRelationshipsXml =
        """<?xml version="1.0" encoding="UTF-8" standalone="yes"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/><Relationship Id="rId2" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/></Relationships>""";

    private const string StylesXml =
        """<?xml version="1.0" encoding="UTF-8" standalone="yes"?><styleSheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"><fonts count="2"><font><sz val="11"/><name val="Calibri"/></font><font><b/><sz val="11"/><name val="Calibri"/></font></fonts><fills count="2"><fill><patternFill patternType="none"/></fill><fill><patternFill patternType="gray125"/></fill></fills><borders count="1"><border><left/><right/><top/><bottom/><diagonal/></border></borders><cellStyleXfs count="1"><xf numFmtId="0" fontId="0" fillId="0" borderId="0"/></cellStyleXfs><cellXfs count="3"><xf numFmtId="0" fontId="0" fillId="0" borderId="0" xfId="0"/><xf numFmtId="49" fontId="1" fillId="0" borderId="0" xfId="0" applyFont="1" applyNumberFormat="1"/><xf numFmtId="49" fontId="0" fillId="0" borderId="0" xfId="0" applyNumberFormat="1"/></cellXfs><cellStyles count="1"><cellStyle name="Normal" xfId="0" builtinId="0"/></cellStyles></styleSheet>""";

    private static readonly UTF8Encoding Utf8WithoutBom = new(encoderShouldEmitUTF8Identifier: false);

    public string ContentType => XlsxContentType;

    public string FileExtension => XlsxFileExtension;

    public async Task WriteAsync(
        Stream destination,
        IReadOnlyList<string> headers,
        IEnumerable<IReadOnlyList<string?>> rows,
        CancellationToken cancellationToken)
    {
        var archive = await ZipArchive.CreateAsync(destination, ZipArchiveMode.Create, leaveOpen: true, entryNameEncoding: null, cancellationToken)
            .ConfigureAwait(false);
        await using (archive.ConfigureAwait(false))
        {
            await WritePartAsync(archive, SpreadsheetMl.ContentTypesPath, ContentTypesXml, cancellationToken).ConfigureAwait(false);
            await WritePartAsync(archive, SpreadsheetMl.PackageRelationshipsPath, PackageRelationshipsXml, cancellationToken).ConfigureAwait(false);
            await WritePartAsync(archive, SpreadsheetMl.DefaultWorkbookPath, WorkbookXml, cancellationToken).ConfigureAwait(false);
            await WritePartAsync(archive, SpreadsheetMl.WorkbookRelationshipsPath, WorkbookRelationshipsXml, cancellationToken).ConfigureAwait(false);
            await WritePartAsync(archive, SpreadsheetMl.StylesPath, StylesXml, cancellationToken).ConfigureAwait(false);

            var worksheetStream = await archive.CreateEntry(SpreadsheetMl.WorksheetPath).OpenAsync(cancellationToken).ConfigureAwait(false);
            await using (worksheetStream.ConfigureAwait(false))
            {
                await WriteWorksheetAsync(worksheetStream, headers, rows).ConfigureAwait(false);
            }
        }
    }

    private static async Task WritePartAsync(ZipArchive archive, string path, string xml, CancellationToken cancellationToken)
    {
        var partStream = await archive.CreateEntry(path).OpenAsync(cancellationToken).ConfigureAwait(false);
        await using (partStream.ConfigureAwait(false))
        {
            await partStream.WriteAsync(Utf8WithoutBom.GetBytes(xml), cancellationToken).ConfigureAwait(false);
        }
    }

    private static async Task WriteWorksheetAsync(Stream destination, IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string?>> rows)
    {
        var settings = new XmlWriterSettings { Async = true, Encoding = Utf8WithoutBom, NewLineHandling = NewLineHandling.Entitize };
        var writer = XmlWriter.Create(destination, settings);
        await using (writer.ConfigureAwait(false))
        {
            await writer.WriteStartDocumentAsync(standalone: true).ConfigureAwait(false);
            await writer.WriteStartElementAsync(null, WorksheetElement, SpreadsheetMl.MainNamespace).ConfigureAwait(false);
            await writer.WriteRawAsync(FrozenHeaderRowXml).ConfigureAwait(false);
            if (headers.Count > 0)
            {
                await writer.WriteRawAsync(string.Format(CultureInfo.InvariantCulture, TextColumnsXmlFormat, headers.Count)).ConfigureAwait(false);
            }

            await writer.WriteStartElementAsync(null, SheetDataElement, SpreadsheetMl.MainNamespace).ConfigureAwait(false);
            var rowNumber = FirstRowNumber;
            await WriteRowAsync(writer, rowNumber, headers, HeaderStyleIndex).ConfigureAwait(false);
            foreach (var row in rows)
            {
                await WriteRowAsync(writer, ++rowNumber, row, TextStyleIndex).ConfigureAwait(false);
            }

            await writer.WriteEndElementAsync().ConfigureAwait(false);
            await writer.WriteEndElementAsync().ConfigureAwait(false);
            await writer.WriteEndDocumentAsync().ConfigureAwait(false);
        }
    }

    private static async Task WriteRowAsync(XmlWriter writer, int rowNumber, IReadOnlyList<string?> cells, string styleIndex)
    {
        var rowReference = rowNumber.ToString(CultureInfo.InvariantCulture);
        await writer.WriteStartElementAsync(null, SpreadsheetMl.RowElement, SpreadsheetMl.MainNamespace).ConfigureAwait(false);
        await writer.WriteAttributeStringAsync(null, SpreadsheetMl.ReferenceAttribute, null, rowReference).ConfigureAwait(false);
        for (var columnIndex = 0; columnIndex < cells.Count; columnIndex++)
        {
            var text = RemoveCharactersInvalidInXml(cells[columnIndex]);
            if (text.Length == 0)
            {
                continue;
            }

            await writer.WriteStartElementAsync(null, SpreadsheetMl.CellElement, SpreadsheetMl.MainNamespace).ConfigureAwait(false);
            await writer.WriteAttributeStringAsync(null, SpreadsheetMl.ReferenceAttribute, null, CellReference.ColumnName(columnIndex) + rowReference).ConfigureAwait(false);
            await writer.WriteAttributeStringAsync(null, SpreadsheetMl.StyleAttribute, null, styleIndex).ConfigureAwait(false);
            await writer.WriteAttributeStringAsync(null, SpreadsheetMl.CellTypeAttribute, null, SpreadsheetMl.InlineStringCellType).ConfigureAwait(false);
            await writer.WriteStartElementAsync(null, SpreadsheetMl.InlineStringElement, SpreadsheetMl.MainNamespace).ConfigureAwait(false);
            await writer.WriteStartElementAsync(null, SpreadsheetMl.TextElement, SpreadsheetMl.MainNamespace).ConfigureAwait(false);
            await writer.WriteAttributeStringAsync(XmlPrefix, SpaceAttribute, null, PreserveSpace).ConfigureAwait(false);
            await writer.WriteStringAsync(text).ConfigureAwait(false);
            await writer.WriteEndElementAsync().ConfigureAwait(false);
            await writer.WriteEndElementAsync().ConfigureAwait(false);
            await writer.WriteEndElementAsync().ConfigureAwait(false);
        }

        await writer.WriteEndElementAsync().ConfigureAwait(false);
    }

    private static string RemoveCharactersInvalidInXml(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(value.Length);
        for (var index = 0; index < value.Length; index++)
        {
            if (char.IsSurrogatePair(value, index))
            {
                builder.Append(value, index, 2);
                index++;
            }
            else if (XmlConvert.IsXmlChar(value[index]))
            {
                builder.Append(value[index]);
            }
        }

        return builder.ToString();
    }
}
