namespace ClassManager.ImportExport.Xlsx;

internal static class SpreadsheetMl
{
    public const string MainNamespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

    public const string ContentTypesPath = "[Content_Types].xml";
    public const string PackageRelationshipsPath = "_rels/.rels";
    public const string DefaultWorkbookPath = "xl/workbook.xml";
    public const string WorkbookRelationshipsPath = "xl/_rels/workbook.xml.rels";
    public const string WorksheetPath = "xl/worksheets/sheet1.xml";
    public const string StylesPath = "xl/styles.xml";
    public const string DefaultSharedStringsFileName = "sharedStrings.xml";
    public const string DefaultStylesFileName = "styles.xml";
    public const string RelationshipsFolder = "_rels/";
    public const string RelationshipsExtension = ".rels";

    public const string OfficeDocumentRelationshipType = "/officeDocument";
    public const string SharedStringsRelationshipType = "/sharedStrings";
    public const string StylesRelationshipType = "/styles";

    public const string WorkbookPropertiesElement = "workbookPr";
    public const string SheetElement = "sheet";
    public const string RelationshipElement = "Relationship";
    public const string SharedStringElement = "si";
    public const string PhoneticRunElement = "rPh";
    public const string TextElement = "t";
    public const string ValueElement = "v";
    public const string FormulaElement = "f";
    public const string RowElement = "row";
    public const string CellElement = "c";
    public const string InlineStringElement = "is";
    public const string NumberFormatElement = "numFmt";
    public const string CellFormatsElement = "cellXfs";
    public const string CellFormatElement = "xf";

    public const string IdAttribute = "Id";
    public const string RelationshipIdAttribute = "id";
    public const string TypeAttribute = "Type";
    public const string TargetAttribute = "Target";
    public const string ReferenceAttribute = "r";
    public const string CellTypeAttribute = "t";
    public const string StyleAttribute = "s";
    public const string NumberFormatIdAttribute = "numFmtId";
    public const string FormatCodeAttribute = "formatCode";
    public const string Date1904Attribute = "date1904";

    public const string SharedStringCellType = "s";
    public const string InlineStringCellType = "inlineStr";
    public const string FormulaStringCellType = "str";
    public const string BooleanCellType = "b";
    public const string ErrorCellType = "e";
    public const string DateCellType = "d";

    public const string TrueValue = "1";
    public const string TrueText = "TRUE";
    public const string FalseText = "FALSE";
}
