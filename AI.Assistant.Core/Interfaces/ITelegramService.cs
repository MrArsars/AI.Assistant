namespace AI.Assistant.Core.Interfaces;

public interface ITelegramService
{
    Task SendMessageAsync(long chatId, string text);
}