namespace AI.Assistant.Bot.Services.Interfaces;

public interface ITelegramService
{
    Task SendMessageAsync(long chatId, string text);
}