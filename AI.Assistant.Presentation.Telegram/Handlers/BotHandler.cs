using AI.Assistant.Application.Handlers;
using AI.Assistant.Presentation.Bot.Extensions;
using AI.Assistant.Core.Extensions;
using AI.Assistant.Core.Interfaces;
using AI.Assistant.Core.Models;
using Telegram.Bot.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AI.Assistant.Presentation.Bot.Handlers;

public class BotHandler(MessageHandler handler)
{

    public async Task HandleMessageAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if(update.Message is null) throw new NullReferenceException(nameof(update.Message));
        
        var (chatId, text) = update.Message;

        var reply = await handler.HandleMessageAsync(chatId, text, MessageSource.Telegram);
        
        foreach (var chunk in reply.ChunkBy())
            await botClient.SendMessage(chatId, chunk, cancellationToken: cancellationToken);
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