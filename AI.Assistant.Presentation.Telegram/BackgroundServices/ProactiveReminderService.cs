using AI.Assistant.Application.Services;
using Microsoft.Extensions.Hosting;

namespace AI.Assistant.Presentation.Bot.BackgroundServices;

public class ProactiveReminderService(ReminderService reminderService): BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            //TODO: implement
            // var reminders = await reminderService.GetNeededRemindersAsync();
            //
            // foreach (var reminder in reminders)
            // {
            //     await chatService.SendMessageAsync(reminder.ChatId, reminder.Message, reminder.MessageSource.ToMessageSource());
            //     if (reminder.Id.HasValue)
            //         await reminderService.UpdateReminderAsync(reminder);
            // }

            await Task.Delay(TimeSpan.FromMinutes(1), ct);
        }
    }
}