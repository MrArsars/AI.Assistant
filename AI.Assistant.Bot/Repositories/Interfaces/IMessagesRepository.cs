using AI.Assistant.Bot.Models;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Bot.Repositories;

public interface IMessagesRepository
{
    Task SaveMessageAsync(MessageModel message);
    Task<ChatHistory> GetLatestHistoryByChatIdAsync(long chatId, bool useLimit = true);
}