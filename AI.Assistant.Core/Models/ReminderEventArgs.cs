using AI.Assistant.Core.Extensions;

namespace AI.Assistant.Core.Models;

public class ReminderEventArgs(ReminderModel reminder) : EventArgs
{
    public ReminderModel Reminder { get; } = reminder;

    public void Deconstruct(out long chatId, out string reminder, out MessageSource source)
    {
        chatId = Reminder.ChatId;
        reminder = Reminder.Message;
        source = Reminder.MessageSource.ToMessageSource();
    }
}