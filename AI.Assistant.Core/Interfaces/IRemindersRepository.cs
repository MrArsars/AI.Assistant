using AI.Assistant.Core.Models;

namespace AI.Assistant.Core.Interfaces;

public interface IRemindersRepository
{
    Task SaveReminderAsync(ReminderModel reminder);
    Task<IEnumerable<ReminderModel>> GetNeededRemindersAsync();
    Task UpdateReminderAsync(Guid reminderId, bool? isActive = null, DateTime? nextRunAt = null);
}