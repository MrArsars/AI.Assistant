using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Bot.Extensions;

public static class ChatHistoryExtensions
{
    public static void AddSystemMessages(this ChatHistory chatHistory, IEnumerable<string>? messages)
    {
        if (messages == null) return;
        
        foreach (var message in messages)
        {
            chatHistory.AddSystemMessage(message);
        }
    }
}