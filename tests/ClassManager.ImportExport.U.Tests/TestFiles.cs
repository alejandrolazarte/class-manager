using System.Text;

namespace ClassManager.ImportExport.U.Tests;

internal static class TestFiles
{
    public const string NameKey = "name";
    public const string BirthDateKey = "birthDate";
    public const string PhoneKey = "phone";
    public const string NameHeader = "Alumno";
    public const string BirthDateHeader = "Fecha de nacimiento";
    public const string PhoneHeader = "Teléfono";

    public static readonly IReadOnlyList<ImportColumn> Columns =
    [
        new(NameKey, NameHeader, ColumnType.Text, IsRequired: true) { Aliases = ["nombre", "student"] },
        new(BirthDateKey, BirthDateHeader, ColumnType.Date) { Aliases = ["nacimiento"] },
        new(PhoneKey, PhoneHeader, ColumnType.Phone, IsRequired: true) { Aliases = ["movil", "whatsapp"] },
    ];

    public static byte[] Utf8(string text) => Encoding.UTF8.GetBytes(text);

    public static byte[] Utf8WithBom(string text) => [.. Encoding.UTF8.GetPreamble(), .. Encoding.UTF8.GetBytes(text)];

    public static byte[] Windows1252(string text) =>
        CodePagesEncodingProvider.Instance.GetEncoding(1252)!.GetBytes(text);

    public static TabularData Read(string text)
    {
        var result = new CsvTabularReader().Read(Utf8(text));
        return result.Data ?? throw new InvalidOperationException(result.Error!.Message);
    }

    public static Task<ImportParseResult> ParseAsync(string text, ImportLimits? limits = null) =>
        ParseAsync(Utf8(text), limits);

    public static Task<ImportParseResult> ParseAsync(byte[] content, ImportLimits? limits = null)
    {
        var parser = new ImportParser(new CsvTabularReader());
        return parser.ParseAsync(new MemoryStream(content), Columns, limits ?? ImportLimits.Default, CancellationToken.None);
    }

    public static async Task<string> WriteAsync(IReadOnlyList<string> headers, params IReadOnlyList<string?>[] rows)
    {
        using var destination = new MemoryStream();
        await new CsvTabularWriter().WriteAsync(destination, headers, rows, CancellationToken.None);
        return Encoding.UTF8.GetString(destination.ToArray());
    }
}
