using AI.Assistant.Bot.Repositories;
using AI.Assistant.Bot.Services.Interfaces;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Bot.Services;

public class HistoryService(IMessagesRepository messagesRepository, Settings settings) : IHistoryService
{
    public async Task<ChatHistory> Initialize(long chatId)
    {
        var initializedHistory = new ChatHistory(settings.SystemPrompt);
        var latestHistory = await messagesRepository.GetLatestHistoryByChatIdAsync(chatId);
        initializedHistory.AddRange(latestHistory);
        return initializedHistory;
    }
    
}