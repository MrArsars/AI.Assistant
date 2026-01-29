using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AI.Assistant.Bot.Models;

[Table("Reminders")]
public class ReminderModel : BaseModel
{
    [PrimaryKey("id", false)] public Guid? Id { get; set; }
    [Column("chat_id")] public long ChatId { get; set; }
    [Column("message")] public string Message { get; set; } = string.Empty;
    [Column("recurrence_rule")] public string? ReccurenceRule { get; set; }
    [Column("next_run_at")] public DateTime NextRunAt { get; set; }
    [Column("is_active")] public bool IsActive { get; set; }
    [Column("created_at")] public DateTime CreatedAt { get; set; }

    public ReminderModel()
    {
    }

    public ReminderModel(long chatId, string message, string? recurrentRule, DateTime nextRunAt)
    {
        ChatId = chatId;
        Message = message;
        ReccurenceRule = recurrentRule;
        NextRunAt = nextRunAt;
        IsActive = true;
    }
}