using AI.Assistant.Application.Interfaces;
using AI.Assistant.Core.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;

namespace AI.Assistant.Infrastructure.Services;

public class AiService(
    Kernel kernel,
    IChatCompletionService chatCompletionService,
    GeminiPromptExecutionSettings geminiPromptExecutionSettings) : IAiService
{
    public async Task<string> GetAiResponse(ChatHistory history, long chatId, MessageSource source)
    {
        kernel.Data["chatId"] = chatId;
        kernel.Data["history"] = history;
        kernel.Data["source"] = source;

        var result = await chatCompletionService.GetChatMessageContentAsync(
            history,
            kernel: kernel,
            executionSettings: geminiPromptExecutionSettings);

        var reply = result.Content ?? "Вибач, сталася помилка.";
        return reply;
    }
}