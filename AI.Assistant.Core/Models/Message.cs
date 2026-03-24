namespace AI.Assistant.Core.Models;

public class Message
{
    public Guid? Id { get; set; }
    public long ChatId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public float[]? Embedding { get; set; }

    public Message()
    {
    }

    public Message(long chatId, string role, string text, float[]? embedding)
    {
        ChatId = chatId;
        Role = role;
        Text = text;
        CreatedAt = DateTime.Now;
        Embedding = embedding;
    }
}