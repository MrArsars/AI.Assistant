using AI.Assistant.Bot.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel.ChatCompletion;
using Telegram.Bot;

namespace AI.Assistant.Bot.Services;

public class ProactiveReminderService(
    IChatService chatService,
    IReminderService reminderService,
    IHistoryService historyService)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var reminders = await reminderService.GetNeededRemindersAsync();

            foreach (var reminder in reminders)
            {
                var history = await historyService.GetHistoryByChatId(reminder.ChatId);
                
                await chatService.SendMessageAsync(reminder.ChatId, reminder.Message);
                await historyService.SaveMessageAsync(reminder.Message, reminder.ChatId, history, AuthorRole.Assistant);
                if (reminder.Id.HasValue)
                    await reminderService.UpdateReminderAsync(reminder);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), ct);
        }
    }
}