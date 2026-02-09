using System.Text;
using AI.Assistant.Core.Extensions;
using AI.Assistant.Core.Interfaces;
using AI.Assistant.Core.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;

namespace AI.Assistant.Core.Services;

public class ChatService(
    IHistoryService historyService,
    Kernel kernel,
    IChatCompletionService chatCompletionService,
    GeminiPromptExecutionSettings geminiPromptExecutionSettings,
    ISenderService senderService) : IChatService
{
    public async Task HandleIncomingMessageAsync(long chatId, MessageSource source = MessageSource.Telegram)
    {
        var history = await historyService.GetHistoryByChatId(chatId);
        historyService.UpdateLocalTimeAsync(history);

        kernel.Data["chatId"] = chatId;
        kernel.Data["history"] = history;
        kernel.Data["source"] = source;
        
        await historyService.TrimHistoryIfNeeded(history, chatId);

        var result = await chatCompletionService.GetChatMessageContentAsync(
            history,
            kernel: kernel,
            executionSettings: geminiPromptExecutionSettings);

        var reply = result.Content ?? "Вибач, сталася помилка.";
        
        await SendMessageAsync(chatId, reply, source);
    }

    public static string LoadSystemInstruction()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Prompts", "system_instruction.txt");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Промпт не знайдено за шляхом: {filePath}");

        return File.ReadAllText(filePath, Encoding.UTF8);
    }

    public async Task SendMessageAsync(long chatId, string text, MessageSource source)
    {
        foreach (var chunk in text.ChunkBy())
            await senderService.SendMessageAsync(chatId, chunk, source);
    }
}