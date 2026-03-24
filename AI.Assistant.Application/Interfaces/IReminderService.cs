using AI.Assistant.Core.Models;

namespace AI.Assistant.Application.Interfaces;

public interface IReminderService
{
    Task CreateReminderAsync(Reminder reminder);
    Task<IEnumerable<Reminder>> GetNeededRemindersAsync();
    Task UpdateReminderAsync(Reminder reminder);
}