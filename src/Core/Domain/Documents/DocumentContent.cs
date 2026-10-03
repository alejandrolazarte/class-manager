namespace ClassManager.Core.Domain.Documents;

public sealed record DocumentContent(byte[] Content, string ContentType, string FileExtension);
