using AI.Assistant.Core.Providers;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Core.Extensions;

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
    
    public static ChatHistory UpdateLocalTime(this ChatHistory history)
    {
        var dateTimeInstruction = DateTimeProvider.DateTimeNow;
        history[1].Content = dateTimeInstruction;
        return history;
    }
}