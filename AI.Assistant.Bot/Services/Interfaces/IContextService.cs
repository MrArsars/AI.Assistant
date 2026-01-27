using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Bot.Services.Interfaces;

public interface IContextService
{
    Task SaveContextAsync(long chatId, ChatHistory chatHistory, string info);
    Task<List<string>> GetContextByChatIdAsync(long chatId);
}