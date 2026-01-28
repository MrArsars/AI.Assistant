using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AI.Assistant.Bot.Models;

public class ReminderModel
{
     public Guid? Id { get; set; }
     public long ChatId { get; set; }
     public string Message { get; set; } = string.Empty;
     public string? ReccurenceRule { get; set; }
     public bool IsActive { get; set; }
     
     public ReminderModel(){}

     public ReminderModel(ReminderModelDto reminder)
     {
          Id = reminder.Id;
          ChatId = reminder.ChatId;
          Message = reminder.Message;
          ReccurenceRule = reminder.ReccurenceRule;
          IsActive = reminder.IsActive;
     }
}