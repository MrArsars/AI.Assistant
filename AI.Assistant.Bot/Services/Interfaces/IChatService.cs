using Microsoft.SemanticKernel.ChatCompletion;
using Telegram.Bot.Types;

namespace AI.Assistant.Bot.Services.Interfaces;

public interface IChatService
{
    Task InitializeHistoryWithContextAsync(long chatId, Dictionary<long, ChatHistory> historiesCollection);
    Task HandleIncomingMessageAsync(ChatHistory history, Message message);  
    void TrimHistory(ChatHistory history);
}