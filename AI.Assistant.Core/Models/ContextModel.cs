using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AI.Assistant.Core.Models;

[Table("Context")]
public class ContextModel : BaseModel
{
    [PrimaryKey("id", false)] public Guid? Id { get; set; }
    [Column("chat_id")] public long ChatId { get; set; }
    [Column("text")] public string Text { get; set; } = string.Empty;
    [Column("created_at")] public DateTime CreatedAt { get; set; }

    public ContextModel()
    {
    }

    public ContextModel(long chatId, string info)
    {
        ChatId = chatId;
        Text = info;
        CreatedAt = DateTime.Now;
    }
}