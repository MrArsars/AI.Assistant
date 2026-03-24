using AI.Assistant.Core.Extensions;

namespace AI.Assistant.Core.Models;

public class ReminderEventArgs(Reminder reminder) : EventArgs
{
    public Reminder Reminder { get; } = reminder;

    public void Deconstruct(out long chatId, out string reminder, out MessageSource source)
    {
        chatId = Reminder.ChatId;
        reminder = Reminder.Message;
        source = Reminder.MessageSource.ToMessageSource();
    }
}