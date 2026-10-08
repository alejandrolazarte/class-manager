using ClassManager.Core.Common;
using ClassManager.Core.Domain.Documents;
using ClassManager.Storage.Documents;

namespace ClassManager.Core.Domain.ClassGroups;

public static class ClassMaterialFile
{
    public const int MaximumSizeInBytes = 20 * 1024 * 1024;

    private const string UnsupportedFileMessage = "The material must be a PDF file.";
    private const string FileTooLargeMessage = "The material must be 20 MB or smaller.";
    private const string ContentFieldName = "File";

    public static Result<DocumentContent> Validate(byte[] content)
    {
        if (!PdfFormat.IsPdf(content))
        {
            return Result.Validation<DocumentContent>(UnsupportedFileMessage, ClassGroupErrorCodes.MaterialUnsupported, ContentFieldName);
        }

        if (content.Length > MaximumSizeInBytes)
        {
            return Result.Validation<DocumentContent>(FileTooLargeMessage, ClassGroupErrorCodes.MaterialTooLarge, ContentFieldName);
        }

        return new DocumentContent(content, PdfFormat.ContentType, PdfFormat.FileExtension);
    }
}
