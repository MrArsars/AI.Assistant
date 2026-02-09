namespace AI.Assistant.Core.Interfaces;

public interface IChatService
{
    Task HandleIncomingMessageAsync(long chatId);  
    Task SendMessageAsync(long chatId, string text);
}