using AI.Assistant.Core.Interfaces;
using Microsoft.SemanticKernel.ChatCompletion;
using Telegram.Bot;

namespace AI.Assistant.Core.Services;

public class TelegramService(ITelegramBotClient telegramBotClient, IHistoryService historyService) : ITelegramService
{
    public async Task SendMessageAsync(long chatId, string text)
    {
        await telegramBotClient.SendMessage(chatId, text);
        await historyService.AddMessageAsync(chatId, text, AuthorRole.Assistant);
    }
}