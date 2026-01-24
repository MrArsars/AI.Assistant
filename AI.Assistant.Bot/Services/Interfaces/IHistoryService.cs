using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Bot.Services;

public interface IHistoryService
{
    Task<ChatHistory> Initialize(long chatId);
}