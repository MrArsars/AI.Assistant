using AI.Assistant.Core.Interfaces;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Core.Services;

public class ContextService(IContextRepository contextRepository) : IContextService
{
    public async Task SaveContextAsync(long chatId, ChatHistory chatHistory, string info)
    {
        await contextRepository.SaveContextAsync(chatId, info);
        chatHistory.AddSystemMessage($"[MEMORY UPDATED]: {info}");
    }

    public async Task<List<string>> GetContextByChatIdAsync(long chatId)
    {
        var context = await contextRepository.GetContextByChatIdAsync(chatId);
        return context;
    }
}