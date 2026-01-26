using AI.Assistant.Bot.Models;
using AI.Assistant.Bot.Repositories;
using AI.Assistant.Bot.Repositories.Interfaces;
using AI.Assistant.Bot.Services.Interfaces;

namespace AI.Assistant.Bot.Services;

public class ContextService(IContextRepository contextRepository) : IContextService
{
    public async Task SaveContextAsync(long chatId, string info)
    {
        await contextRepository.SaveContextAsync(chatId, info);
    }

    public async Task<List<string>> GetContextByChatIdAsync(long chatId)
    {
        var context = await contextRepository.GetContextByChatIdAsync(chatId);
        return context;
    }
}