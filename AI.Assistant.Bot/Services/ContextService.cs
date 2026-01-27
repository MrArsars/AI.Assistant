using AI.Assistant.Bot.Models;
using AI.Assistant.Bot.Repositories;
using AI.Assistant.Bot.Repositories.Interfaces;
using AI.Assistant.Bot.Services.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Bot.Services;

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