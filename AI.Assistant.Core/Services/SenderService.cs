using AI.Assistant.Core.Interfaces;
using AI.Assistant.Core.Models;
using Microsoft.SemanticKernel.ChatCompletion;
using Telegram.Bot;

namespace AI.Assistant.Core.Services;

public class SenderService(ITelegramBotClient telegramBotClient, IHistoryService historyService) : ISenderService
{
    public async Task SendMessageAsync(long chatId, string text, MessageSource source)
    {
        await telegramBotClient.SendMessage(chatId, text);
        await historyService.AddMessageAsync(chatId, text, AuthorRole.Assistant);
    }
}