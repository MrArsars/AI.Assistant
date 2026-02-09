using AI.Assistant.Core.Extensions;
using AI.Assistant.Core.Interfaces;
using Microsoft.Extensions.Hosting;

namespace AI.Assistant.Bot.BackgroundServices;

public class ProactiveReminderService(
    IChatService chatService,
    IReminderService reminderService)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var reminders = await reminderService.GetNeededRemindersAsync();

            foreach (var reminder in reminders)
            {
                await chatService.SendMessageAsync(reminder.ChatId, reminder.Message, reminder.MessageSource.ToMessageSource());
                if (reminder.Id.HasValue)
                    await reminderService.UpdateReminderAsync(reminder);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), ct);
        }
    }
}