using AI.Assistant.Core.Interfaces;
using AI.Assistant.Core.Models;

namespace AI.Assistant.Core.Services;

public class ReminderService(IRemindersRepository remindersRepository) : IReminderService
{
    public async Task CreateReminderAsync(long chatId, string message, string? recurrenceRule, DateTime nextRunAt, MessageSource source)
    {
        var newReminder = new ReminderModel(chatId, message, recurrenceRule, nextRunAt, source);
        await remindersRepository.SaveReminderAsync(newReminder);
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