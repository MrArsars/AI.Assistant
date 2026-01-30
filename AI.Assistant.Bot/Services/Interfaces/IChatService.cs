using Microsoft.SemanticKernel.ChatCompletion;
using Telegram.Bot.Types;

namespace AI.Assistant.Bot.Services.Interfaces;

public interface IChatService
{
    Task HandleIncomingMessageAsync(ChatHistory history, Message message);  
    Task SendMessageAsync(long chatId, string text);
}