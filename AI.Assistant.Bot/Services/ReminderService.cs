using AI.Assistant.Bot.Models;
using AI.Assistant.Bot.Repositories.Interfaces;
using AI.Assistant.Bot.Services.Interfaces;

namespace AI.Assistant.Bot.Services;

public class ReminderService(IRemindersRepository remindersRepository) : IReminderService
{
    public async Task CreateReminderAsync(long chatId, string message, string? recurrenceRule, DateTime nextRunAt)
    {
        var newReminder = new ReminderModel(chatId, message, recurrenceRule, nextRunAt);
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
        bool isActive = reminder.IsActive;
        DateTime nextRunAt = reminder.NextRunAt;

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