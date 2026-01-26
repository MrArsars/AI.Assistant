using Telegram.Bot.Exceptions;
using AI.Assistant.Bot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;

namespace AI.Assistant.Bot.Handlers;

public class BotHandler(
    Kernel kernel,
    IChatService chatService)
{
    //TODO: ConcurrentDictionary
    private Dictionary<long, ChatHistory> _historiesCollection = new();

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        var msg = update.Message;
        if (msg?.Text is null) return;

        if (!_historiesCollection.ContainsKey(msg.Chat.Id))
            await chatService.InitializeHistoryWithContextAsync(msg.Chat.Id, _historiesCollection);
        
        var history = _historiesCollection[msg.Chat.Id];

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