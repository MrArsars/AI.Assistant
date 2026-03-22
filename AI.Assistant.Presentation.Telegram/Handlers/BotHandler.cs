using AI.Assistant.Application.Handlers;
using AI.Assistant.Application.Interfaces;
using AI.Assistant.Core.Extensions;
using AI.Assistant.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace AI.Assistant.Presentation.Telegram.Handlers;

public class BotHandler(MessageHandler handler, ISanitizerAgent sanitizerAgent)
{
    public async Task HandleMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        if (update.Message is not { } message) return;

        var rawText = message switch
        {
            { Text: not null } => message.Text,
            { Voice: not null } => await GetTranscriptAsync(message.Voice.FileId, botClient, ct),
            _ => throw new Exception("Error: empty message")
        };

        if (rawText.StartsWith('/'))
        {
            await HandleCommandAsync(message.Chat.Id, rawText, botClient, ct);
            return;
        }

        await ProcessAiFlowAsync(message.Chat.Id, rawText, message.Voice != null ? "Voice" : "Text", botClient, ct);
    }

    private async Task ProcessAiFlowAsync(long chatId, string rawText, string type, ITelegramBotClient botClient,
        CancellationToken ct)
    {
        var sanitized = await sanitizerAgent.SanitizeAsync(rawText, type, ct);
        var reply = await handler.GenerateResponseAsync(chatId, sanitized.Content, MessageSource.Telegram, ct);

        await SendLargeMessageAsync(chatId, reply, botClient, ct);
    }

    private async Task HandleCommandAsync(long chatId, string command, ITelegramBotClient botClient,
        CancellationToken ct)
    {
        var response = command switch
        {
            "/start" => await handler.Introduce(chatId, MessageSource.Telegram),
            _ => "Невідома команда"
        };
        await botClient.SendMessage(chatId, response, cancellationToken: ct);
    }

    private async Task<string> GetTranscriptAsync(string voiceId, ITelegramBotClient botClient, CancellationToken ct)
    {
        var file = await botClient.GetFile(voiceId, ct);
        if (file.FilePath == null) return string.Empty;

        using var ms = new MemoryStream();
        await botClient.DownloadFile(file.FilePath, ms, ct);
        ms.Position = 0;

        return await handler.TranscriptVoiceMessage(ms, ct);
    }

    private async Task SendLargeMessageAsync(long chatId, string text, ITelegramBotClient botClient,
        CancellationToken ct)
    {
        foreach (var chunk in text.ChunkBy())
        {
            await botClient.SendMessage(chatId, chunk, cancellationToken: ct);
        }
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