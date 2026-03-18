using AI.Assistant.Application.Handlers;
using AI.Assistant.Application.Interfaces;
using AI.Assistant.Core.Extensions;
using AI.Assistant.Core.Models;
using AI.Assistant.Presentation.Telegram.Extensions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace AI.Assistant.Presentation.Telegram.Handlers;

public class BotHandler(MessageHandler handler, ISanitizerAgent sanitizerAgent)
{
    public async Task HandleMessageAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message is null) throw new NullReferenceException(nameof(update.Message));

        if (update.Message.Text is not null)
            await HandleTextAsync(update.Message, botClient, cancellationToken);

        if (update.Message.Voice is not null)
            await HandleVoiceAsync(update.Message, botClient, cancellationToken);
    }

    private async Task HandleTextAsync(Message msg, ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        var (chatId, text, _) = msg;

        if (text == null) return;

        var sanitizedMessage = await sanitizerAgent.SanitizeAsync(text, "Text", cancellationToken);

        var reply = text switch
        {
            "/start" => await handler.Introduce(chatId, MessageSource.Telegram),
            _ => await handler.GenerateResponseAsync(chatId, sanitizedMessage.Content, MessageSource.Telegram)
        };

        foreach (var chunk in reply.ChunkBy())
            await botClient.SendMessage(chatId, chunk, cancellationToken: cancellationToken);
    }

    private async Task HandleVoiceAsync(Message msg, ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        var (chatId, _, voiceId) = msg;
        if (voiceId == null) return;

        var memoryStream = await GetFileStream(voiceId, botClient, cancellationToken);
        var message = await handler.TranscriptVoiceMessage(memoryStream, cancellationToken);

        var sanitizedMessage = await sanitizerAgent.SanitizeAsync(message, "Voice", cancellationToken);

        var reply = await handler.GenerateResponseAsync(chatId, sanitizedMessage.Content, MessageSource.Telegram);

        foreach (var chunk in reply.ChunkBy())
            await botClient.SendMessage(chatId, chunk, cancellationToken: cancellationToken);
    }

    private async Task<Stream> GetFileStream(string voiceId, ITelegramBotClient botClient,
        CancellationToken cancellationToken)
    {
        var voiceMessage = await botClient.GetFile(voiceId, cancellationToken);

        if (voiceMessage.FilePath == null) throw new Exception("File is not existing");

        var memoryStream = new MemoryStream();
        await botClient.DownloadFile(voiceMessage.FilePath, memoryStream, cancellationToken);
        memoryStream.Position = 0;

        return memoryStream;
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