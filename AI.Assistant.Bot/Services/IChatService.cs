using Microsoft.SemanticKernel.ChatCompletion;
using Telegram.Bot.Types;

namespace AI.Assistant.Bot.Services;

public interface IChatService
{
    void TrimHistory(ChatHistory history);
    Task SaveMessageAsync(long chatId, AuthorRole role, string text);
    Task<ChatHistory> LoadHistoryAsync(long chatId);
    Task SavePermanentAsync(long chatId, string info);
}