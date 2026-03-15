using Telegram.Bot.Types;

namespace AI.Assistant.Presentation.Telegram.Extensions;

public static class MessageExtensions
{
    public static void Deconstruct(this Message msg, out long chatId, out string? text, out string? voiceId)
    {
        chatId = msg.Chat.Id;
        text = msg.Text;
        voiceId = msg.Voice?.FileId;
    }
}