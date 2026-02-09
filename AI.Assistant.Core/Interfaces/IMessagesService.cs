using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Core.Interfaces;

public interface IMessagesService
{
    Task SaveToRepositoryAsync(string text, long chatId, AuthorRole role);
    Task<ChatHistory> GetLatestHistoryByChatIdAsync(long chatId, bool useLimit = true);
}