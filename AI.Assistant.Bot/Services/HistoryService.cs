using AI.Assistant.Bot.Extensions;
using AI.Assistant.Bot.Models;
using AI.Assistant.Bot.Repositories.Interfaces;
using AI.Assistant.Bot.Services.Interfaces;
using Microsoft.SemanticKernel.ChatCompletion;
using Telegram.Bot.Types;

namespace AI.Assistant.Bot.Services;

public class HistoryService(IMessagesRepository messagesRepository, IContextService contextService, Settings settings)
    : IHistoryService
{
    private readonly Dictionary<long, ChatHistory> _historiesCollection = new();

    public async Task<ChatHistory?> Initialize(long chatId, ChatHistory? history = null)
    {
        var dateTimeInstruction = GetCurrentTime();
        var context = await contextService.GetContextByChatIdAsync(chatId);
        var latestHistory = await messagesRepository.GetLatestHistoryByChatIdAsync(chatId);
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

    public async Task SaveMessageAsync(Message message, ChatHistory history, AuthorRole role)
    {
        history.AddMessage(role, message.Text!);
        var messageModel = new MessageModel(message.Chat.Id, role.Label, message.Text!);
        await messagesRepository.SaveMessageAsync(messageModel);
    }

    public async Task SaveMessageAsync(string text, long chatId, ChatHistory history, AuthorRole role)
    {
        history.AddMessage(role, text);
        var messageModel = new MessageModel(chatId, role.Label, text);
        await messagesRepository.SaveMessageAsync(messageModel);
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