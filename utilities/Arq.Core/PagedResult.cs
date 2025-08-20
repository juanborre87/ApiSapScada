namespace Arq.Core;

public class PagedResult<T>
{
    public MetaData MetaData { get; set; } = new MetaData();
    public List<T> Data { get; set; } = new();
}
