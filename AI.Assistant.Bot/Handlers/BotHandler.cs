using AI.Assistant.Bot.Services;
using Microsoft.SemanticKernel.Connectors.Google;

namespace AI.Assistant.Bot.Handlers;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;

public class BotHandler(
    ITelegramBotClient botClient,
    IChatCompletionService chatCompletion,
    Kernel kernel,
    IChatService  chatService,
    string systemPrompt)
{
    //TODO: ConcurrentDictionary
    private Dictionary<long, ChatHistory> _historiesCollection = new();

    public async Task HandleUpdateAsync(Message msg, UpdateType type)
    {
        if (msg.Text is null) return;
        
        Console.WriteLine($"Received {type} '{msg.Text}' from {msg.Chat.Id}");
        Console.WriteLine($"Plugins in kernel: {kernel.Plugins.Count}");
        if (!_historiesCollection.ContainsKey(msg.Chat.Id))
        {
            _historiesCollection.Add(msg.Chat.Id, new ChatHistory(systemPrompt));
            Console.WriteLine($"Created new history with chatId {msg.Chat.Id}");
            await GetLatestMessagesAsync(msg.Chat.Id, _historiesCollection[msg.Chat.Id]);
            await GetPermanentMemoriesAsync(msg.Chat.Id, _historiesCollection[msg.Chat.Id]);
        }

        await botClient.SendChatAction(msg.Chat.Id, ChatAction.Typing);

        var history = _historiesCollection[msg.Chat.Id];
        chatService.TrimHistory(history);
        history.AddUserMessage(msg.Text);
        await chatService.SaveMessageAsync(msg.Chat.Id, AuthorRole.User, msg.Text);
        
        var promptExecutionSettings = new GeminiPromptExecutionSettings() 
        { 
            ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,
        };
        
        kernel.Data["chatId"] =  msg.Chat.Id;
        kernel.Data["history"] = history;
        var result = await chatCompletion.GetChatMessageContentAsync(
            history,
            kernel: kernel,
            executionSettings: promptExecutionSettings);
        var reply = result.Content ?? "Вибач, сталася помилка.";

        history.AddAssistantMessage(reply);
        await chatService.SaveMessageAsync(msg.Chat.Id, AuthorRole.Assistant, reply);
        await botClient.SendMessage(msg.Chat.Id, reply);
    }

    private async Task GetLatestMessagesAsync(long chatId, ChatHistory history)
    {
        var latestMessages = await chatService.LoadHistoryAsync(chatId);
        var receivedMessagesCount =  latestMessages.Count;
        history.AddRange(latestMessages);
        Console.WriteLine($"Got {receivedMessagesCount} history messages from DB on chatId {chatId}");
    }

    private async Task GetPermanentMemoriesAsync(long chatId, ChatHistory history)
    {
        var memories = await chatService.GetPermanentMemoriesAsync(chatId);
        foreach (var message in memories)
        {
            history.AddSystemMessage(message);
        }
    }
}