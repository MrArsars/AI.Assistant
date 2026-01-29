using AI.Assistant.Bot.Models;

namespace AI.Assistant.Bot.Repositories.Interfaces;

public interface IRemindersRepository
{
    Task SaveReminderAsync(ReminderModel reminder);
    Task<IEnumerable<ReminderModel>> GetNeededRemindersAsync();
    Task UpdateReminderAsync(Guid reminderId, bool? isActive = null, DateTime? nextRunAt = null);
}