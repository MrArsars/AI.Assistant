using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Core.Interfaces;

public interface IContextService
{
    Task SaveContextAsync(long chatId, ChatHistory chatHistory, string info);
    Task<List<string>> GetContextByChatIdAsync(long chatId);
}