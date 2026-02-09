using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AI.Assistant.Core.Models;

[Table("Messages")]
public class MessageModel : BaseModel
{
    [PrimaryKey("id", false)] public Guid? Id { get; set; }
    [Column("chat_id")] public long ChatId { get; set; }
    [Column("role")] public string Role { get; set; } = string.Empty;
    [Column("text")] public string Text { get; set; } = string.Empty;
    [Column("created_at")] public DateTime CreatedAt { get; set; }
    
    public MessageModel(){}

    public MessageModel(long chatId, string role, string text)
    {
        ChatId = chatId;
        Role = role;
        Text = text;
        CreatedAt = DateTime.Now;
    }
}