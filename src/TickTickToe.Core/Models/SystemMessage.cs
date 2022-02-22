namespace TickTickToe.Core.Models;

public record SystemMessage
{
    public string Text { get; init; } = string.Empty;
    public MessageSeverity Severity { get; init; }
    public enum MessageSeverity
    {
        Success,
        Info,
        Warning,
        Error
    }
}