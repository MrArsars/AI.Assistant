using AI.Assistant.Bot.Extensions;
using AI.Assistant.Bot.Services.Interfaces;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Bot.Services;

public class HistoryService(IContextService contextService, IMessagesService messagesService, Settings settings)
    : IHistoryService
{
    private readonly Dictionary<long, ChatHistory> _historiesCollection = new();

    public async Task<ChatHistory?> Initialize(long chatId, ChatHistory? history = null)
    {
        var dateTimeInstruction = GetCurrentTime();
        var context = await contextService.GetContextByChatIdAsync(chatId);
        var latestHistory = await messagesService.GetLatestHistoryByChatIdAsync(chatId);
        var isReinitializing = false;

        if (history == null)
            history = [];
        else
            isReinitializing = true;
        
        history.AddSystemMessage(settings.SystemPrompt);
        history.AddSystemMessage(dateTimeInstruction);
        history.AddSystemMessages(context);
        history.AddRange(latestHistory);

        if (isReinitializing) return null;
        _historiesCollection.Add(chatId, history);
        return history;
    }
    

    public void UpdateLocalTimeAsync(ChatHistory chatHistory)
    {
        var dateTimeInstruction = GetCurrentTime();
        chatHistory[1].Content = dateTimeInstruction;
    }

    public async Task AddMessageAsync(long chatId, string text, AuthorRole role)
    {
        var history = await GetHistoryByChatId(chatId);
        history.AddMessage(role, text);
        await messagesService.SaveToRepositoryAsync(text, chatId, role);
    }

    public async Task<ChatHistory> GetHistoryByChatId(long chatId)
    {
        var history = _historiesCollection.ContainsKey(chatId) switch
        {
            true => _historiesCollection[chatId],
            false => await Initialize(chatId)
        };

        return history;
    }

    public async Task TrimHistoryIfNeeded(ChatHistory history, long chatId)
    {
        if (history.Count(x => x.Role == AuthorRole.User) > settings.HistoryMaxLimit)
        {
            history.Clear();
            await Initialize(chatId, history);
        }
    }

    private static string GetCurrentTime()
    {
        return $"Поточний час: {DateTime.Now}";
    }
}