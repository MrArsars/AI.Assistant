using AI.Assistant.Bot.Models;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Bot.Services.Interfaces;

public interface IMessagesService
{
    Task SaveToRepositoryAsync(string text, long chatId, AuthorRole role);
    Task<ChatHistory> GetLatestHistoryByChatIdAsync(long chatId, bool useLimit = true);
}