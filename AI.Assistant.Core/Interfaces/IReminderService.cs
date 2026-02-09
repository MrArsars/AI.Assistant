using AI.Assistant.Core.Models;

namespace AI.Assistant.Core.Interfaces;

public interface IReminderService
{
    Task CreateReminderAsync(long chatId, string message, string? recurrenceRule, DateTime nextRunAt, MessageSource source);
    Task<IEnumerable<ReminderModel>> GetNeededRemindersAsync();
    Task UpdateReminderAsync(ReminderModel reminder);
}