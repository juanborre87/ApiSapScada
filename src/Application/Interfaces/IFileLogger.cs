namespace Application.Interfaces;

public interface IFileLogger
{
    // Async methods
    Task LogInfoAsync(string message, string method = null);
    Task LogWarningAsync(string message, string method = null);
    Task LogErrorAsync(string message, Exception? ex = null, string method = null);
    Task LogErrorAsync(string message, string method = null);

    // Synchronous methods
    void LogInfo(string message, string method = null);
    void LogWarning(string message, string method = null);
    void LogError(string message, Exception? ex = null, string method = null);
    void LogError(string message, string method = null);
}
