using AI.Assistant.Application.Interfaces;
using AI.Assistant.Core.Interfaces;
using AI.Assistant.Core.Models;

namespace AI.Assistant.Application.Services;

public class ReminderService(IRemindersRepository remindersRepository)
{
    public async Task CreateReminderAsync(ReminderModel reminder)
    {
        await remindersRepository.SaveReminderAsync(reminder);
    }

    public async Task<IEnumerable<ReminderModel>> GetNeededRemindersAsync()
    {
        var result = await remindersRepository.GetNeededRemindersAsync();
        return result;
    }

    public async Task UpdateReminderAsync(ReminderModel reminder)
    {
        var reminderId = reminder.Id;
        var isActive = reminder.IsActive;
        var nextRunAt = reminder.NextRunAt;

        switch (reminder.ReccurenceRule)
        {
            case "none":
                isActive = false;
                break;
            case "daily":
                nextRunAt = nextRunAt.AddDays(1);
                break;
            case "weekly":
                nextRunAt = nextRunAt.AddDays(7);
                break;
        }

        await remindersRepository.UpdateReminderAsync((Guid)reminderId!, isActive, nextRunAt);
    }
}