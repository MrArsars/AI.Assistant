namespace AI.Assistant.Bot.Services.Interfaces;

public interface IContextService
{
    Task SaveContextAsync(long chatId, string info);
    Task<List<string>> GetContextByChatIdAsync(long chatId);
}