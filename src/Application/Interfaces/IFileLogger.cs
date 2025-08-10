namespace Application.Interfaces
{
    public interface IFileLogger
    {
        Task LogInfoAsync(string message);
        Task LogWarningAsync(string message);
        Task LogErrorAsync(string message, Exception? ex = null);
    }
}
