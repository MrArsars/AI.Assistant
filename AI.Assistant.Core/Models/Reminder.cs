namespace AI.Assistant.Core.Models;

public class Reminder
{
    public Guid? Id { get; set; }
    public long ChatId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ReccurenceRule { get; set; }
    public DateTime NextRunAt { get; set; }
    public bool IsActive { get; set; }
    public string? MessageSource { get; set; }
    public DateTime CreatedAt { get; set; }

    public Reminder()
    {
    }

    public Reminder(long chatId, string message, string? recurrentRule, DateTime nextRunAt, MessageSource source)
    {
        ChatId = chatId;
        Message = message;
        ReccurenceRule = recurrentRule;
        NextRunAt = nextRunAt;
        IsActive = true;
        MessageSource = source.ToString();
        CreatedAt = DateTime.Now;
    }
}