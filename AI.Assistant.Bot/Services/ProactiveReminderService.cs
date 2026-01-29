using AI.Assistant.Bot.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace AI.Assistant.Bot.Services;

public class ProactiveReminderService(ITelegramBotClient telegramBotClient, IReminderService reminderService)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var reminders = await reminderService.GetNeededRemindersAsync();

            foreach (var reminder in reminders)
            {
                await telegramBotClient.SendMessage(reminder.ChatId, reminder.Message, cancellationToken: ct);
                if (reminder.Id.HasValue)
                    await reminderService.UpdateReminderAsync(reminder);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), ct);
        }
    }
}