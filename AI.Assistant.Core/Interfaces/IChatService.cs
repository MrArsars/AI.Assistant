using AI.Assistant.Core.Models;

namespace AI.Assistant.Core.Interfaces;

public interface IChatService
{
    Task HandleIncomingMessageAsync(long chatId, MessageSource source);  
    Task SendMessageAsync(long chatId, string text, MessageSource source);
}