namespace AI.Assistant.Core.Models;

public class Context
{
    public Guid? Id { get; set; }
    public long ChatId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Context()
    {
    }

    public Context(long chatId, string info)
    {
        ChatId = chatId;
        Text = info;
        CreatedAt = DateTime.Now;
    }
}