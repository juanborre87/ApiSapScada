using Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class FileLogger : IFileLogger
    {
        private readonly string _logDirectory;

        public FileLogger(IConfiguration configuration)
        {
            // Extrae la ruta desde appsettings.json en Logging:FilePath
            _logDirectory = configuration["FileLogger:FilePath"]
                ?? throw new ArgumentNullException("FileLogger:FilePath no está configurado en appsettings.json");

            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public Task LogInfoAsync(string message) =>
            WriteLogAsync("INFO", message);

        public Task LogWarningAsync(string message) =>
            WriteLogAsync("WARNING", message);

        public Task LogErrorAsync(string message, Exception? ex = null)
        {
            var errorMessage = ex == null ? message : $"{message}{Environment.NewLine}{ex}";
            return WriteLogAsync("ERROR", errorMessage);
        }

        private async Task WriteLogAsync(string level, string message)
        {
            string logFileName = $"log_{DateTime.Now:yyyyMMdd}.txt";
            string logFilePath = Path.Combine(_logDirectory, logFileName);

            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}{Environment.NewLine}";
            await File.AppendAllTextAsync(logFilePath, logEntry);
        }
    }
}
