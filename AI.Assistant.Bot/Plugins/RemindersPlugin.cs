using System.ComponentModel;
using AI.Assistant.Bot.Repositories.Interfaces;
using AI.Assistant.Bot.Services.Interfaces;
using Microsoft.SemanticKernel;

namespace AI.Assistant.Bot.Plugins;

public class RemindersPlugin(IReminderService reminderService)
{
    [KernelFunction("create_reminder")]
    [Description(
        "Schedules a new reminder or a proactive check-in. Use this when the user asks to be reminded about something," +
        " wants to set up a daily report, plans a future task or requests for delayed message from you in future.")]
    public async Task CreateReminderAsync(
        [Description(
            "A friendly, conversational, and personalized text of notification text that will be sent to the user in the user's language. E.g., 'Time for your daily progress report!'")]
        string message,
        [Description("The exact date and time for the reminder. Example: 2026-01-29T21:00:00")]
        DateTime nextRunAt,
        [Description("Frequency: 'none'(for one-time reminder), 'daily', or 'weekly'. Default is 'none'.")] 
        string recurrenceRule,
        Kernel kernel
    )
    {
        if (kernel.Data.TryGetValue("chatId", out var objId) && objId is long chatId)
        {
            await reminderService.CreateReminderAsync(chatId, message, recurrenceRule, nextRunAt);
        }
    }
}