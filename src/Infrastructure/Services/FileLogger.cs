using Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class FileLogger : IFileLogger
{
    private readonly string _logDirectory;

    public FileLogger(IConfiguration configuration)
    {
        _logDirectory = configuration["FileLogger:FilePath"]
            ?? throw new ArgumentNullException("FileLogger:FilePath no está configurado en appsettings.json");

        if (!Directory.Exists(_logDirectory))
            Directory.CreateDirectory(_logDirectory);
    }

    // Async methods
    public Task LogInfoAsync(string message, string method = null) =>
        WriteLogAsync("INFO", message, method);

    public Task LogWarningAsync(string message, string method = null) =>
        WriteLogAsync("WARNING", message, method);

    public Task LogErrorAsync(string message, Exception? ex = null, string method = null)
    {
        var errorMessage = ex == null ? message : $"{message}{Environment.NewLine}{ex}";
        return WriteLogAsync("ERROR", errorMessage, method);
    }

    public Task LogErrorAsync(string message, string method = null) =>
        WriteLogAsync("ERROR", message, method);

    private async Task WriteLogAsync(string level, string message, string method)
    {
        string logFileName = $"log_{DateTime.Now:yyyyMMdd}.txt";
        string logFilePath = Path.Combine(_logDirectory, logFileName);

        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}  {method}{Environment.NewLine}";
        await File.AppendAllTextAsync(logFilePath, logEntry);
    }

    // Synchronous methods
    public void LogInfo(string message, string method = null) =>
        WriteLog("INFO", message, method);

    public void LogWarning(string message, string method = null) =>
        WriteLog("WARNING", message, method);

    public void LogError(string message, Exception? ex = null, string method = null)
    {
        var errorMessage = ex == null ? message : $"{message}{Environment.NewLine}{ex}";
        WriteLog("ERROR", errorMessage, method);
    }

    public void LogError(string message, string method = null) =>
        WriteLog("ERROR", message, method);

    private void WriteLog(string level, string message, string method)
    {
        string logFileName = $"log_{DateTime.Now:yyyyMMdd}.txt";
        string logFilePath = Path.Combine(_logDirectory, logFileName);

        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}  {method}{Environment.NewLine}";
        File.AppendAllText(logFilePath, logEntry);
    }
}
