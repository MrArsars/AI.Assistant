using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AI.Assistant.Bot.Models;

[Table("Reminders")]

public class ReminderModelDto: BaseModel
{
    [PrimaryKey("id", false)] public Guid? Id { get; set; }
    [Column("chat_id")] public long ChatId { get; set; }
    [Column("message")] public string Message { get; set; } = string.Empty;
    [Column("recurrence_rule")] public string? ReccurenceRule { get; set; }
    [Column("next_run_at")] public DateTime? NextRunAt { get; set; }
    [Column("is_active")] public bool IsActive { get; set; }
    [Column("created_at")] public DateTime CreatedAt { get; set; }
    
    public ReminderModelDto(){}

    public ReminderModelDto(ReminderModel reminder)
    {
        Id = reminder.Id;
        ChatId = reminder.ChatId;
        Message = reminder.Message;
        ReccurenceRule = reminder.ReccurenceRule;
        IsActive = reminder.IsActive;
    }
}