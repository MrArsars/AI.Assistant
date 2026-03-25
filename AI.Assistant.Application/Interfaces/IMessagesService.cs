using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Application.Interfaces;

public interface IMessagesService
{
    Task SaveToRepositoryAsync(string text, long chatId, AuthorRole role, float[]? embedding);
    Task<ChatHistory> GetLatestHistoryByChatIdAsync(long chatId, bool useLimit = true);
}