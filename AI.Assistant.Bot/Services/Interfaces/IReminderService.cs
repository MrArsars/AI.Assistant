using AI.Assistant.Bot.Models;

namespace AI.Assistant.Bot.Services.Interfaces;

public interface IReminderService
{
    Task CreateReminderAsync(long chatId, string message, string? recurrenceRule, DateTime nextRunAt);
    Task<IEnumerable<ReminderModel>> GetNeededRemindersAsync();
    Task UpdateReminderAsync(ReminderModel reminder);
}