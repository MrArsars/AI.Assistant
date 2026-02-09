using System.Text;
using AI.Assistant.Core.Extensions;
using AI.Assistant.Core.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;

namespace AI.Assistant.Core.Services;

public class ChatService(
    IHistoryService historyService,
    IMessagesService messagesService,
    Kernel kernel,
    IChatCompletionService chatCompletionService,
    GeminiPromptExecutionSettings geminiPromptExecutionSettings,
    ITelegramService telegramService) : IChatService
{
    public async Task HandleIncomingMessageAsync(long chatId)
    {
        var history = await historyService.GetHistoryByChatId(chatId);
        historyService.UpdateLocalTimeAsync(history);

        kernel.Data["chatId"] = chatId;
        kernel.Data["history"] = history;
        
        await historyService.TrimHistoryIfNeeded(history, chatId);

        var result = await chatCompletionService.GetChatMessageContentAsync(
            history,
            kernel: kernel,
            executionSettings: geminiPromptExecutionSettings);

        var reply = result.Content ?? "Вибач, сталася помилка.";
        
        await SendMessageAsync(chatId, reply);
    }

    public static string LoadSystemInstruction()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Prompts", "system_instruction.txt");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Промпт не знайдено за шляхом: {filePath}");

        return File.ReadAllText(filePath, Encoding.UTF8);
    }

    public async Task SendMessageAsync(long chatId, string text)
    {
        foreach (var chunk in text.ChunkBy())
            await telegramService.SendMessageAsync(chatId, chunk);
    }
}