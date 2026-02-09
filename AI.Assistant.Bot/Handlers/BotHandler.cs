using AI.Assistant.Core.Interfaces;
using AI.Assistant.Core.Models;
using Telegram.Bot.Exceptions;
using Microsoft.SemanticKernel.ChatCompletion;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AI.Assistant.Bot.Handlers;

public class BotHandler(IChatService chatService, IHistoryService historyService)
{

    public async Task HandleMessageAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        var msg = update.Message;
        if (msg?.Text is null) return;

        await historyService.AddMessageAsync(msg.Chat.Id, msg.Text, AuthorRole.User);
        await chatService.HandleIncomingMessageAsync(msg.Chat.Id, MessageSource.Telegram);
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        await Task.CompletedTask;
    }
}