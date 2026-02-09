namespace AI.Assistant.Core.Interfaces;

public interface IContextRepository
{
    Task SaveContextAsync(long chatId, string info);
    Task<List<string>> GetContextByChatIdAsync(long chatId);
}