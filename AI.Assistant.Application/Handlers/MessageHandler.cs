using AI.Assistant.Application.Interfaces;
using AI.Assistant.Core.Interfaces;
using AI.Assistant.Core.Models;
using Microsoft.SemanticKernel.ChatCompletion;
using static AI.Assistant.Core.Prompts.Prompts;

namespace AI.Assistant.Application.Handlers;

public class MessageHandler(
    IHistoryService historyService,
    IAiService aiService,
    IVoiceTranscriptionService voiceTranscriptionService
)
{
    public async Task<string> GenerateResponseAsync(long chatId, string message, MessageSource source)
    {
        var history = await historyService.GetHistoryByChatId(chatId);
        await historyService.TrimHistoryIfNeeded(history, chatId);
        await historyService.AddMessageAsync(chatId, message, AuthorRole.User);

        var reply = await aiService.GetAiResponse(history, chatId, source);

        await historyService.AddMessageAsync(chatId, reply, AuthorRole.Assistant);

        return reply;
    }

    public async Task<string> TranscriptVoiceMessage(Stream memoryStream, CancellationToken cancellationToken)
    {
        var message = await voiceTranscriptionService.TranscriptVoiceMessage(memoryStream, cancellationToken);

        return message;
    }

    public async Task AddProactiveToHistoryAsync(long chatId, string message)
    {
        await historyService.AddMessageAsync(chatId, SystemSeparator, AuthorRole.System);
        await historyService.AddMessageAsync(chatId, message, AuthorRole.Assistant);
    }

    public async Task<string> Introduce(long chatId, MessageSource source)
    {
        _ = await historyService.Initialize(chatId);
        var introduceMessage = Introduction;
        await historyService.AddMessageAsync(chatId, introduceMessage, AuthorRole.Assistant);
        return introduceMessage;
    }
}