using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Bot.Services.Interfaces;

public interface IHistoryService
{
    Task<ChatHistory> Initialize(long chatId);
}