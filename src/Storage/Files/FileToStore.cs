namespace ClassManager.Storage.Files;

public sealed record FileToStore(string Path, ReadOnlyMemory<byte> Content, string ContentType);
