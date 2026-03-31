using AI.Assistant.Core.Models;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Application.Interfaces;

public interface IHistoryService
{
    Task<ChatHistory?> Initialize(long chatId, ChatHistory? history = null);

    Task AddMessageAsync(long chatId, string text, AuthorRole role, MessageType? type = null,
        float[]? embedding = null);

    Task<ChatHistory> GetHistoryByChatId(long chatId);
    Task TrimHistoryIfNeeded(ChatHistory history, long chatId);
}