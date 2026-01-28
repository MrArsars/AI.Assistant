using AI.Assistant.Bot.Models;

namespace AI.Assistant.Bot.Repositories.Interfaces;

public interface IRemindersRepository
{
    Task SaveReminderAsync(ReminderModelDto reminder);
    Task<IEnumerable<ReminderModel>> GetNeededRemindersAsync();
    Task UpdateReminder(Guid reminderId, bool? isActive = null, DateTime? nextRunAt = null);
}