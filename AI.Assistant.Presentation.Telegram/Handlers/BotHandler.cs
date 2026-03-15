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

        if (update.Message.Text is not null)
            await HandleTextAsync(update.Message, botClient, cancellationToken);

        if (update.Message.Voice is not null)
            await HandleVoiceAsync(update.Message, botClient, cancellationToken);
    }

    private async Task HandleTextAsync(Message msg, ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        var (chatId, text, _) = msg;

        if (text == null) return;

        var reply = text switch
        {
            "/start" => await handler.Introduce(chatId, MessageSource.Telegram),
            _ => await handler.GenerateResponseAsync(chatId, text, MessageSource.Telegram)
        };

        foreach (var chunk in reply.ChunkBy())
            await botClient.SendMessage(chatId, chunk, cancellationToken: cancellationToken);
    }

    private async Task HandleVoiceAsync(Message msg, ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        var (chatId, _, voiceId) = msg;
        if (voiceId == null) return;

        var filePath = await DownloadVoiceMessage(voiceId, botClient, cancellationToken);
        var message = await handler.TranscriptVoiceMessage(filePath, cancellationToken);

        var reply = await handler.GenerateResponseAsync(chatId, message, MessageSource.Telegram);

        foreach (var chunk in reply.ChunkBy())
            await botClient.SendMessage(chatId, chunk, cancellationToken: cancellationToken);
    }

    private async Task<string> DownloadVoiceMessage(string voiceId, ITelegramBotClient botClient,
        CancellationToken cancellationToken)
    {
        var voiceMessage = await botClient.GetFile(voiceId, cancellationToken);

        var directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
        var filePath = Path.Combine(directoryPath, "voice.oga");

        Directory.CreateDirectory(directoryPath);

        await using (FileStream fileStream = new(filePath, FileMode.Create))
        {
            if (voiceMessage.FilePath != null)
                await botClient.DownloadFile(voiceMessage.FilePath, fileStream, cancellationToken);
        }

        return filePath;
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