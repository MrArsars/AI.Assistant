namespace AI.Assistant.Application.Interfaces;

public interface IContextManager
{
    Task SaveContextAsync(long chatId, string info);
}