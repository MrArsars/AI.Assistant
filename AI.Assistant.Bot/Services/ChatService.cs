using System.Text;
using AI.Assistant.Bot.Extensions;
using AI.Assistant.Bot.Models;
using AI.Assistant.Bot.Repositories;
using AI.Assistant.Bot.Services.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AI.Assistant.Bot.Services;

public class ChatService(
    IContextService contextService,
    IHistoryService historyService,
    Kernel kernel,
    IChatCompletionService chatCompletionService,
    IMessagesRepository messagesRepository,
    GeminiPromptExecutionSettings geminiPromptExecutionSettings,
    ITelegramBotClient telegramBotClient) : IChatService
{
    public async Task InitializeHistoryWithContextAsync(long chatId, Dictionary<long, ChatHistory> historiesCollection)
    {
        var initializedHistory = await historyService.Initialize(chatId);
        historiesCollection.Add(chatId, initializedHistory);
        var context = await contextService.GetContextByChatIdAsync(chatId);
        initializedHistory.AddSystemMessages(context);
    }

    public async Task HandleIncomingMessageAsync(ChatHistory history, Message message)
    {
        historyService.UpdateLocalTimeAsync(history);
        history.AddUserMessage(message.Text!);
        var userMessage = new MessageModel(message.Chat.Id, AuthorRole.User.Label, message.Text!);
        await messagesRepository.SaveMessageAsync(userMessage);

        kernel.Data["chatId"] = message.Chat.Id;
        kernel.Data["history"] = history;

        var result = await chatCompletionService.GetChatMessageContentAsync(
            history,
            kernel: kernel,
            executionSettings: geminiPromptExecutionSettings);

        var reply = result.Content ?? "Вибач, сталася помилка.";

        history.AddAssistantMessage(reply);
        var assistantMessage = new MessageModel(message.Chat.Id, AuthorRole.Assistant.Label, reply);
        await messagesRepository.SaveMessageAsync(assistantMessage);

        await SendMessageAsync(message.Chat.Id, reply);
    }

    //TODO:Review method
    public void TrimHistory(ChatHistory history)
    {
        // while (history.Count > historyLimit)
        // {
        //     if (history.Count > 1) history.RemoveRange(1, Math.Min(2, history.Count - 1));
        //     else break;
        // }
    }

    public static string LoadSystemInstruction()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Prompts", "system_instruction.txt");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Промпт не знайдено за шляхом: {filePath}");

        return File.ReadAllText(filePath, Encoding.UTF8);
    }

    private async Task SendMessageAsync(long chatId, string text)
    {
        foreach (var chunk in text.ChunkBy())
            await telegramBotClient.SendMessage(chatId, chunk);
    }
}