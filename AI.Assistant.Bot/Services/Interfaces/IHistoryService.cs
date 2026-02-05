using Microsoft.SemanticKernel.ChatCompletion;
using Telegram.Bot.Types;

namespace AI.Assistant.Bot.Services.Interfaces;

public interface IHistoryService
{
    Task<ChatHistory?> Initialize(long chatId, ChatHistory? history = null);
    void UpdateLocalTimeAsync(ChatHistory chatHistory);
    Task AddMessageAsync(long chatId, string text, AuthorRole role);
    Task<ChatHistory> GetHistoryByChatId(long chatId);
    Task TrimHistoryIfNeeded(ChatHistory history, long chatId);
}