using System.IO.Compression;
using System.Text;

namespace ClassManager.ImportExport.U.Tests;

internal static class XlsxFiles
{
    private const string WorkbookXml =
        """<workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"><sheets><sheet name="Datos" sheetId="1" r:id="rId7"/></sheets></workbook>""";

    private const string WorkbookRelationshipsXml =
        """<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId7" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="/xl/worksheets/datos.xml"/></Relationships>""";

    public static async Task<byte[]> WriteAsync(IReadOnlyList<string> headers, params IReadOnlyList<string?>[] rows)
    {
        using var destination = new MemoryStream();
        await new XlsxTabularWriter().WriteAsync(destination, headers, rows, CancellationToken.None);
        return destination.ToArray();
    }

    public static byte[] Workbook(string sheetDataXml, string? sharedStringsXml = null, string? stylesXml = null)
    {
        var parts = new Dictionary<string, string>
        {
            ["xl/workbook.xml"] = WorkbookXml,
            ["xl/_rels/workbook.xml.rels"] = WorkbookRelationshipsXml,
            ["xl/worksheets/datos.xml"] =
                $"""<worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"><sheetData>{sheetDataXml}</sheetData></worksheet>""",
        };
        if (sharedStringsXml is not null)
        {
            parts["xl/sharedStrings.xml"] = $"""<sst xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">{sharedStringsXml}</sst>""";
        }

        if (stylesXml is not null)
        {
            parts["xl/styles.xml"] = $"""<styleSheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">{stylesXml}</styleSheet>""";
        }

        return Zip(parts);
    }

    public static byte[] Zip(IReadOnlyDictionary<string, string> parts)
    {
        using var content = new MemoryStream();
        using (var archive = new ZipArchive(content, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var (path, xml) in parts)
            {
                using var entryStream = archive.CreateEntry(path).Open();
                entryStream.Write(Encoding.UTF8.GetBytes(xml));
            }
        }

        return content.ToArray();
    }

    public static TabularData Read(byte[] content)
    {
        var result = new XlsxTabularReader().Read(content);
        return result.Data ?? throw new InvalidOperationException(result.Error!.Message);
    }
}
