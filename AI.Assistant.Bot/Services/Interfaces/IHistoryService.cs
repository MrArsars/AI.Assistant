using Microsoft.SemanticKernel.ChatCompletion;
using Telegram.Bot.Types;

namespace AI.Assistant.Bot.Services.Interfaces;

public interface IHistoryService
{
    Task<ChatHistory> Initialize(long chatId, ChatHistory? history = null);
    void UpdateLocalTimeAsync(ChatHistory chatHistory);
    Task SaveMessageAsync(Message message, ChatHistory history, AuthorRole role);
    Task SaveMessageAsync(string text, long chatId, ChatHistory history, AuthorRole role);
    Task<ChatHistory> GetHistoryByChatId(long chatId);
    Task TrimHistoryIfNeeded(ChatHistory history, long chatId);
}