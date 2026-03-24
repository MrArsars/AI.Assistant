using AI.Assistant.Core.Models;

namespace AI.Assistant.Application.Interfaces;

public interface IRemindersRepository
{
    Task SaveReminderAsync(Reminder reminder);
    Task<IEnumerable<Reminder>> GetNeededRemindersAsync();
    Task UpdateReminderAsync(Guid reminderId, bool? isActive = null, DateTime? nextRunAt = null);
}