using AI.Assistant.Application.Extensions;
using AI.Assistant.Application.Interfaces;
using AI.Assistant.Core.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.ChatCompletion;
using static AI.Assistant.Core.Prompts.Prompts;

namespace AI.Assistant.Application.Services;

public class HistoryService(
    IMessagesService messagesService,
    IContextProvider contextProvider,
    IOptions<ApplicationSettings> settings)
    : IHistoryService
{
    private readonly Dictionary<long, ChatHistory> _historiesCollection = new();

    public async Task<ChatHistory?> Initialize(long chatId, ChatHistory? history = null)
    {
        var context = await contextProvider.GetContextByChatIdAsync(chatId);
        var latestHistory = await messagesService.GetLatestHistoryByChatIdAsync(chatId);
        var isReinitializing = false;

        if (history == null)
            history = [];
        else
            isReinitializing = true;

        history.AddSystemMessage(SystemPrompt);
        history.AddSystemMessages(context);
        history.AddRange(latestHistory);

        if (isReinitializing) return null;
        _historiesCollection.Add(chatId, history);
        return history;
    }

    public async Task AddMessageAsync(long chatId, string text, AuthorRole role, float[]? embedding = null)
    {
        var history = await GetHistoryByChatId(chatId);
        history.AddMessage(role, text);
        await messagesService.SaveToRepositoryAsync(text, chatId, role, embedding);
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
        if (history.Count(x => x.Role == AuthorRole.User) > settings.Value.HistoryMaxLimit)
        {
            history.Clear();
            await Initialize(chatId, history);
        }
    }
}