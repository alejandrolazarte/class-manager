using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Xml;
using ClassManager.ImportExport.Tabular;

namespace ClassManager.ImportExport.Xlsx;

public sealed class XlsxTabularReader(int maximumPartSizeInBytes = XlsxTabularReader.DefaultMaximumPartSizeInBytes) : ITabularReader
{
    public const int DefaultMaximumPartSizeInBytes = 16 * 1024 * 1024;
    public const int MaximumColumnCount = 1024;

    private const string IsoDateFormat = "yyyy-MM-dd";
    private const int Date1904OffsetInDays = 1462;
    private const double MinimumDateSerial = 1;
    private const double MaximumDateSerial = 2958465;
    private const string UnreadableFileMessage = "The file could not be read as an Excel workbook (.xlsx).";
    private const string MissingPartMessage = "The workbook has no worksheet.";
    private const string InvalidSharedStringMessage = "A cell points to a shared text that does not exist.";
    private const string EmptyFileMessage = "The file is empty: the first row must contain the column names.";

    public TabularReadResult Read(ReadOnlyMemory<byte> content)
    {
        List<TabularRow> rows;
        try
        {
            using var archive = new ZipArchive(new MemoryStream(content.ToArray(), writable: false), ZipArchiveMode.Read);
            rows = ReadFirstWorksheet(new XlsxPackage(archive, maximumPartSizeInBytes));
        }
        catch (Exception exception) when (exception is InvalidDataException or XmlException)
        {
            return TabularReadResult.Failure(new ImportFileError(ImportErrorCodes.UnreadableFile, UnreadableFileMessage));
        }

        var nonBlankRows = rows.Where(row => !row.Cells.All(string.IsNullOrWhiteSpace)).ToList();
        if (nonBlankRows.Count == 0)
        {
            return TabularReadResult.Failure(new ImportFileError(ImportErrorCodes.EmptyFile, EmptyFileMessage));
        }

        var headers = nonBlankRows[0].Cells.Select(header => header.Trim()).ToList();
        return TabularReadResult.Success(new TabularData(headers, nonBlankRows[1..]));
    }

    private static List<TabularRow> ReadFirstWorksheet(XlsxPackage package)
    {
        var workbookPath = package.ReadRelationships(string.Empty)
            .FirstOrDefault(relationship => relationship.Type.EndsWith(SpreadsheetMl.OfficeDocumentRelationshipType, StringComparison.Ordinal))
            ?.TargetPath ?? SpreadsheetMl.DefaultWorkbookPath;
        var (firstSheetRelationshipId, usesDate1904) = ReadWorkbook(package, workbookPath);

        var workbookRelationships = package.ReadRelationships(workbookPath);
        var worksheetPath = workbookRelationships.FirstOrDefault(relationship => relationship.Id == firstSheetRelationshipId)?.TargetPath;
        if (worksheetPath is null || !package.Contains(worksheetPath))
        {
            throw new InvalidDataException(MissingPartMessage);
        }

        var workbookFolder = XlsxPackage.FolderOf(workbookPath);
        var sharedStrings = ReadSharedStrings(package, PartPath(workbookRelationships, SpreadsheetMl.SharedStringsRelationshipType, workbookFolder + SpreadsheetMl.DefaultSharedStringsFileName));
        var dateStyles = ReadDateStyles(package, PartPath(workbookRelationships, SpreadsheetMl.StylesRelationshipType, workbookFolder + SpreadsheetMl.DefaultStylesFileName));

        return ReadRows(package, worksheetPath, new CellValueReader(sharedStrings, dateStyles, usesDate1904));
    }

    private static string PartPath(IReadOnlyList<XlsxRelationship> relationships, string relationshipType, string defaultPath) =>
        relationships.FirstOrDefault(relationship => relationship.Type.EndsWith(relationshipType, StringComparison.Ordinal))?.TargetPath ?? defaultPath;

    private static (string? FirstSheetRelationshipId, bool UsesDate1904) ReadWorkbook(XlsxPackage package, string workbookPath)
    {
        using var reader = package.OpenPart(workbookPath) ?? throw new InvalidDataException(MissingPartMessage);
        string? firstSheetRelationshipId = null;
        var usesDate1904 = false;
        while (reader.Read())
        {
            if (reader.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            if (reader.LocalName == SpreadsheetMl.WorkbookPropertiesElement)
            {
                usesDate1904 = IsTrue(reader.GetAttribute(SpreadsheetMl.Date1904Attribute));
            }
            else if (reader.LocalName == SpreadsheetMl.SheetElement && firstSheetRelationshipId is null)
            {
                firstSheetRelationshipId = RelationshipIdOf(reader);
            }
        }

        return (firstSheetRelationshipId, usesDate1904);
    }

    private static string? RelationshipIdOf(XmlReader reader)
    {
        while (reader.MoveToNextAttribute())
        {
            if (reader.LocalName == SpreadsheetMl.RelationshipIdAttribute && reader.NamespaceURI.Length > 0)
            {
                var relationshipId = reader.Value;
                reader.MoveToElement();
                return relationshipId;
            }
        }

        reader.MoveToElement();
        return null;
    }

    private static List<string> ReadSharedStrings(XlsxPackage package, string sharedStringsPath)
    {
        using var reader = package.OpenPart(sharedStringsPath);
        var sharedStrings = new List<string>();
        if (reader is null)
        {
            return sharedStrings;
        }

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == SpreadsheetMl.SharedStringElement)
            {
                using var sharedStringReader = reader.ReadSubtree();
                sharedStrings.Add(ReadText(sharedStringReader).Text);
            }
        }

        return sharedStrings;
    }

    private static HashSet<int> ReadDateStyles(XlsxPackage package, string stylesPath)
    {
        using var reader = package.OpenPart(stylesPath);
        var dateStyles = new HashSet<int>();
        if (reader is null)
        {
            return dateStyles;
        }

        var customFormatCodes = new Dictionary<int, string>();
        while (reader.Read())
        {
            if (reader.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            if (reader.LocalName == SpreadsheetMl.NumberFormatElement
                && TryParseInteger(reader.GetAttribute(SpreadsheetMl.NumberFormatIdAttribute), out var numberFormatId))
            {
                customFormatCodes[numberFormatId] = reader.GetAttribute(SpreadsheetMl.FormatCodeAttribute) ?? string.Empty;
            }
            else if (reader.LocalName == SpreadsheetMl.CellFormatsElement)
            {
                using var cellFormatsReader = reader.ReadSubtree();
                var styleIndex = 0;
                while (cellFormatsReader.Read())
                {
                    if (cellFormatsReader.NodeType == XmlNodeType.Element && cellFormatsReader.LocalName == SpreadsheetMl.CellFormatElement)
                    {
                        if (TryParseInteger(cellFormatsReader.GetAttribute(SpreadsheetMl.NumberFormatIdAttribute), out var cellNumberFormatId)
                            && XlsxDateFormats.IsDateFormat(cellNumberFormatId, customFormatCodes))
                        {
                            dateStyles.Add(styleIndex);
                        }

                        styleIndex++;
                    }
                }
            }
        }

        return dateStyles;
    }

    private static List<TabularRow> ReadRows(XlsxPackage package, string worksheetPath, CellValueReader cellValueReader)
    {
        using var reader = package.OpenPart(worksheetPath) ?? throw new InvalidDataException(MissingPartMessage);
        var rows = new List<TabularRow>();
        List<string>? cells = null;
        var rowNumber = 0;
        var nextColumnIndex = 0;
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == SpreadsheetMl.RowElement)
            {
                rowNumber = TryParseInteger(reader.GetAttribute(SpreadsheetMl.ReferenceAttribute), out var declaredRowNumber) ? declaredRowNumber : rowNumber + 1;
                cells = [];
                nextColumnIndex = 0;
                if (reader.IsEmptyElement)
                {
                    rows.Add(new TabularRow(rowNumber, cells));
                    cells = null;
                }
            }
            else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == SpreadsheetMl.CellElement && cells is not null)
            {
                var columnIndex = CellReference.ColumnIndex(reader.GetAttribute(SpreadsheetMl.ReferenceAttribute)) ?? nextColumnIndex;
                nextColumnIndex = columnIndex + 1;
                var value = cellValueReader.Read(reader);
                if (columnIndex < MaximumColumnCount)
                {
                    SetCell(cells, columnIndex, value);
                }
            }
            else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == SpreadsheetMl.RowElement && cells is not null)
            {
                rows.Add(new TabularRow(rowNumber, cells));
                cells = null;
            }
        }

        return rows;
    }

    private static void SetCell(List<string> cells, int columnIndex, string value)
    {
        while (cells.Count <= columnIndex)
        {
            cells.Add(string.Empty);
        }

        cells[columnIndex] = value;
    }

    private static (string? Value, string Text) ReadText(XmlReader reader)
    {
        string? value = null;
        var text = new StringBuilder();
        reader.Read();
        while (!reader.EOF)
        {
            if (reader.NodeType != XmlNodeType.Element)
            {
                reader.Read();
            }
            else if (reader.LocalName is SpreadsheetMl.PhoneticRunElement or SpreadsheetMl.FormulaElement)
            {
                reader.Skip();
            }
            else if (reader.LocalName == SpreadsheetMl.TextElement)
            {
                text.Append(reader.ReadElementContentAsString());
            }
            else if (reader.LocalName == SpreadsheetMl.ValueElement)
            {
                value = reader.ReadElementContentAsString();
            }
            else
            {
                reader.Read();
            }
        }

        return (value, text.ToString());
    }

    private static bool TryParseInteger(string? text, out int value) =>
        int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out value);

    private static bool IsTrue(string? text) =>
        text is SpreadsheetMl.TrueValue || string.Equals(text, bool.TrueString, StringComparison.OrdinalIgnoreCase);

    private sealed class CellValueReader(List<string> sharedStrings, HashSet<int> dateStyles, bool usesDate1904)
    {
        public string Read(XmlReader reader)
        {
            var cellType = reader.GetAttribute(SpreadsheetMl.CellTypeAttribute);
            var isDateStyle = TryParseInteger(reader.GetAttribute(SpreadsheetMl.StyleAttribute), out var styleIndex) && dateStyles.Contains(styleIndex);
            if (reader.IsEmptyElement)
            {
                return string.Empty;
            }

            using var cellReader = reader.ReadSubtree();
            var (value, text) = ReadText(cellReader);
            return cellType switch
            {
                SpreadsheetMl.InlineStringCellType => text,
                SpreadsheetMl.SharedStringCellType => SharedString(value),
                SpreadsheetMl.BooleanCellType => IsTrue(value) ? SpreadsheetMl.TrueText : SpreadsheetMl.FalseText,
                SpreadsheetMl.DateCellType => IsoDate(value),
                SpreadsheetMl.FormulaStringCellType or SpreadsheetMl.ErrorCellType => value ?? string.Empty,
                _ => Number(value, isDateStyle),
            };
        }

        private string SharedString(string? value) =>
            TryParseInteger(value, out var index) && index < sharedStrings.Count
                ? sharedStrings[index]
                : throw new InvalidDataException(InvalidSharedStringMessage);

        private static string IsoDate(string? value) =>
            DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime)
                ? dateTime.ToString(IsoDateFormat, CultureInfo.InvariantCulture)
                : value ?? string.Empty;

        private string Number(string? value, bool isDateStyle)
        {
            if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
            {
                return value ?? string.Empty;
            }

            var dateSerial = usesDate1904 ? number + Date1904OffsetInDays : number;
            return isDateStyle && dateSerial is >= MinimumDateSerial and <= MaximumDateSerial
                ? DateTime.FromOADate(dateSerial).ToString(IsoDateFormat, CultureInfo.InvariantCulture)
                : number.ToString(CultureInfo.InvariantCulture);
        }
    }
}
