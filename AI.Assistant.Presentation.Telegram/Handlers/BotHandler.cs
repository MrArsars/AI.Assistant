using AI.Assistant.Application.Handlers;
using AI.Assistant.Core.Extensions;
using AI.Assistant.Core.Models;
using AI.Assistant.Presentation.Telegram.Extensions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace AI.Assistant.Presentation.Telegram.Handlers;

public class BotHandler(MessageHandler handler)
{
    public async Task HandleMessageAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message is null) throw new NullReferenceException(nameof(update.Message));

        var (chatId, text) = update.Message;

        var reply = text switch
        {
            "/start" => await handler.Introduce(chatId, MessageSource.Telegram),
            _ => await handler.HandleMessageAsync(chatId, text, MessageSource.Telegram)
        };

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