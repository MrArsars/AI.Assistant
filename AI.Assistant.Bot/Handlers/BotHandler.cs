using AI.Assistant.Bot.Services;

namespace AI.Assistant.Bot.Handlers;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;

public class BotHandler(
    ITelegramBotClient botClient,
    IChatCompletionService chatCompletion,
    ChatService chatService,
    Kernel kernel,
    string systemPrompt)
{
    // private readonly ChatHistory _history = new(systemPrompt);
    private Dictionary<long, ChatHistory> _historiesCollection = new();

    public async Task HandleUpdateAsync(Message msg, UpdateType type)
    {
        if (msg.Text is null) return;

        Console.WriteLine($"Received {type} '{msg.Text}' from {msg.Chat.Id}");
        if (!_historiesCollection.ContainsKey(msg.Chat.Id))
        {
            _historiesCollection.Add(msg.Chat.Id, new ChatHistory(systemPrompt));
            Console.WriteLine($"Created new history with chatId {msg.Chat.Id}");
            await GetLatestMessagesAsync(msg.Chat.Id, _historiesCollection[msg.Chat.Id]);
        }

        await botClient.SendChatAction(msg.Chat.Id, ChatAction.Typing);

        var history = _historiesCollection[msg.Chat.Id];
        chatService.TrimHistory(history);
        history.AddUserMessage(msg.Text);
        await chatService.SaveMessageAsync(msg.Chat.Id, AuthorRole.User, msg.Text);

        var result = await chatCompletion.GetChatMessageContentAsync(history, kernel: kernel);
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
}