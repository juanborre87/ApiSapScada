namespace Arq.Host;

public class Notify
{
    public bool IsException { get; set; } = false;
    public string Property { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"Notify - Code: {IsException}, Property: {Property}, Message: {Message}";
    }
}