using AI.Assistant.Application.Services;
using AI.Assistant.Core.Models;
using Microsoft.Extensions.Hosting;

namespace AI.Assistant.Application.Publishers;

public class ProactivePublisher(ReminderService reminderService): BackgroundService
{
    public delegate Task AsyncEventHandler<in TEventArgs>(object sender, TEventArgs e);
    public event AsyncEventHandler<ReminderEventArgs>? OnMessageReceived;
    
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var reminders = await reminderService.GetNeededRemindersAsync();
            
            foreach (var reminder in reminders)
            {
                await (OnMessageReceived?.Invoke(this, new ReminderEventArgs(reminder)) ?? Task.CompletedTask);
                
                if (reminder.Id.HasValue)
                    await reminderService.UpdateReminderAsync(reminder);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), ct);
        }
    }
}