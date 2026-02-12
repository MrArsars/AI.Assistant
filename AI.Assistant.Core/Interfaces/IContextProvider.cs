namespace AI.Assistant.Core.Interfaces;

public interface IContextProvider
{
    Task<List<string>> GetContextByChatIdAsync(long chatId);
}