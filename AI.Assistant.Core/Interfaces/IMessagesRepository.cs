using AI.Assistant.Core.Models;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Core.Interfaces;

public interface IMessagesRepository
{
    Task SaveMessageAsync(Message message);
    Task<ChatHistory> GetLatestHistoryByChatIdAsync(long chatId, bool useLimit = true);
}