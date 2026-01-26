namespace AI.Assistant.Bot.Repositories.Interfaces;

public interface IContextRepository
{
    Task SaveContextAsync(long chatId, string info);
    Task<List<string>> GetContextByChatIdAsync(long chatId);
}