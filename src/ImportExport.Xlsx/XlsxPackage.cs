using System.IO.Compression;
using System.Xml;

namespace ClassManager.ImportExport.Xlsx;

internal sealed class XlsxPackage(ZipArchive archive, int maximumPartSizeInBytes)
{
    private const char PathSeparator = '/';
    private const string ParentFolder = "..";
    private const string CurrentFolder = ".";
    private const int ReadBufferSize = 81920;
    private const string PartTooLargeMessage = "A part of the workbook is larger than the limit.";

    private static readonly XmlReaderSettings SafeXmlSettings = new()
    {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = null,
        CloseInput = true,
    };

    public bool Contains(string path) => archive.GetEntry(path) is not null;

    public XmlReader? OpenPart(string path)
    {
        var entry = archive.GetEntry(path);
        if (entry is null)
        {
            return null;
        }

        if (entry.Length > maximumPartSizeInBytes)
        {
            throw new InvalidDataException(PartTooLargeMessage);
        }

        var content = new MemoryStream();
        using (var entryStream = entry.Open())
        {
            var buffer = new byte[ReadBufferSize];
            int bytesRead;
            while ((bytesRead = entryStream.Read(buffer)) > 0)
            {
                if (content.Length + bytesRead > maximumPartSizeInBytes)
                {
                    throw new InvalidDataException(PartTooLargeMessage);
                }

                content.Write(buffer, 0, bytesRead);
            }
        }

        content.Position = 0;
        return XmlReader.Create(content, SafeXmlSettings);
    }

    public IReadOnlyList<XlsxRelationship> ReadRelationships(string partPath)
    {
        var relationshipsPath = RelationshipsPathOf(partPath);
        using var reader = OpenPart(relationshipsPath);
        if (reader is null)
        {
            return [];
        }

        var sourceFolder = FolderOf(partPath);
        var relationships = new List<XlsxRelationship>();
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == SpreadsheetMl.RelationshipElement)
            {
                var target = reader.GetAttribute(SpreadsheetMl.TargetAttribute);
                if (target is not null)
                {
                    relationships.Add(new XlsxRelationship(
                        reader.GetAttribute(SpreadsheetMl.IdAttribute) ?? string.Empty,
                        reader.GetAttribute(SpreadsheetMl.TypeAttribute) ?? string.Empty,
                        ResolveTarget(sourceFolder, target)));
                }
            }
        }

        return relationships;
    }

    public static string FolderOf(string partPath)
    {
        var separatorIndex = partPath.LastIndexOf(PathSeparator);
        return separatorIndex < 0 ? string.Empty : partPath[..(separatorIndex + 1)];
    }

    private static string RelationshipsPathOf(string partPath) =>
        FolderOf(partPath) + SpreadsheetMl.RelationshipsFolder + partPath[FolderOf(partPath).Length..] + SpreadsheetMl.RelationshipsExtension;

    private static string ResolveTarget(string sourceFolder, string target)
    {
        var combined = target.StartsWith(PathSeparator) ? target : sourceFolder + target;
        var segments = new List<string>();
        foreach (var segment in combined.Split(PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            if (segment == ParentFolder)
            {
                if (segments.Count > 0)
                {
                    segments.RemoveAt(segments.Count - 1);
                }
            }
            else if (segment != CurrentFolder)
            {
                segments.Add(segment);
            }
        }

        return string.Join(PathSeparator, segments);
    }
}
