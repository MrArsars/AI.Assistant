using AI.Assistant.Bot.Repositories.Interfaces;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace AI.Assistant.Bot.Services;

public class ProactiveReminderService(ITelegramBotClient telegramBotClient, IRemindersRepository remindersRepository)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var reminders = await remindersRepository.GetNeededRemindersAsync();

            foreach (var reminder in reminders)
            {
                await telegramBotClient.SendMessage(reminder.ChatId, reminder.Message, cancellationToken: ct);
                if (reminder.Id.HasValue)
                    await remindersRepository.UpdateReminder((Guid)reminder.Id, false);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), ct);
        }
    }
}