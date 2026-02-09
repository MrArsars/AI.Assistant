using AI.Assistant.Core.Models;

namespace AI.Assistant.Core.Interfaces;

public interface ISenderService
{
    Task SendMessageAsync(long chatId, string text, MessageSource source);
}