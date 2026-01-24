using AI.Assistant.Bot.Models;
using AI.Assistant.Bot.Repositories;
using AI.Assistant.Bot.Services;
using Microsoft.SemanticKernel.Connectors.Google;
using Telegram.Bot.Exceptions;

namespace AI.Assistant.Bot.Handlers;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;

public class BotHandler(
    IChatCompletionService chatCompletion,
    Kernel kernel,
    IChatService  chatService,
    IMessagesRepository messagesRepository,
    IHistoryService historyService)
{
    //TODO: ConcurrentDictionary
    private Dictionary<long, ChatHistory> _historiesCollection = new();

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var msg = update.Message;
        if (msg.Text is null) return;
        
        Console.WriteLine($"Received {update.Type} '{msg.Text}' from {msg.Chat.Id}");
        Console.WriteLine($"Plugins in kernel: {kernel.Plugins.Count}");
        if (!_historiesCollection.ContainsKey(msg.Chat.Id))
        {
            _historiesCollection.Add(msg.Chat.Id, await historyService.Initialize(msg.Chat.Id));
            await GetPermanentMemoriesAsync(msg.Chat.Id, _historiesCollection[msg.Chat.Id]);
        }

        await botClient.SendChatAction(msg.Chat.Id, ChatAction.Typing);

        var history = _historiesCollection[msg.Chat.Id];
        chatService.TrimHistory(history);
        history.AddUserMessage(msg.Text);
        var model = new MessageModel
        {
            ChatId = msg.Chat.Id,
            Role = AuthorRole.User.ToString(),
            Text = msg.Text,
            CreatedAt = DateTime.Now
        };
        await messagesRepository.SaveMessageAsync(model);
        
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

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        await Task.CompletedTask;
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