using AI.Assistant.Bot.Repositories;
using AI.Assistant.Bot.Services.Interfaces;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Bot.Services;

public class HistoryService(IMessagesRepository messagesRepository, Settings settings) : IHistoryService
{
    public async Task<ChatHistory> Initialize(long chatId)
    {
        var initializedHistory = new ChatHistory(settings.SystemPrompt);
        var dateTimeInstruction = GetCurrentTime();
        initializedHistory.AddSystemMessage(dateTimeInstruction);
        var latestHistory = await messagesRepository.GetLatestHistoryByChatIdAsync(chatId);
        initializedHistory.AddRange(latestHistory);
        return initializedHistory;
    }

    public void UpdateLocalTimeAsync(ChatHistory chatHistory)
    {
        var dateTimeInstruction = GetCurrentTime();
        chatHistory[1].Content = dateTimeInstruction;
    }

    private static string GetCurrentTime()
    {
        return $"Поточний час: {DateTime.Now}";
    }
}