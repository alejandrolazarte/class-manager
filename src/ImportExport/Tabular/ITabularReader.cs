namespace ClassManager.ImportExport.Tabular;

public interface ITabularReader
{
    TabularReadResult Read(ReadOnlyMemory<byte> content);
}
