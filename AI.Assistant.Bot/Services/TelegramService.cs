using AI.Assistant.Bot.Services.Interfaces;
using Microsoft.SemanticKernel.ChatCompletion;
using Telegram.Bot;

namespace AI.Assistant.Bot.Services;

public class TelegramService(ITelegramBotClient telegramBotClient, IHistoryService historyService) : ITelegramService
{
    public async Task SendMessageAsync(long chatId, string text)
    {
        await telegramBotClient.SendMessage(chatId, text);
        await historyService.AddMessageAsync(chatId, text, AuthorRole.Assistant);
    }
}