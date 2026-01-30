using Telegram.Bot.Exceptions;
using AI.Assistant.Bot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Bot.Handlers;

public class BotHandler(IChatService chatService, IHistoryService historyService)
{

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        
        var msg = update.Message;
        if (msg?.Text is null) return;

        var history = await historyService.GetHistoryByChatId(msg.Chat.Id);
        await chatService.HandleIncomingMessageAsync(history, msg);
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