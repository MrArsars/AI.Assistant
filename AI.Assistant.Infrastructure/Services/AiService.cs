using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using AI.Assistant.Application.Interfaces;
using AI.Assistant.Core.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;

namespace AI.Assistant.Infrastructure.Services;

public class AiService(
    Kernel kernel,
    IChatCompletionService chatCompletionService) : IAiService
{
    public async Task<string> GetAiResponse(ChatHistory history, long chatId)
    {
        kernel.Data["chatId"] = chatId;
        kernel.Data["history"] = history;

        var options = JsonSerializerOptions.Default;
        var schema = options.GetJsonSchemaAsNode(typeof(AiResponse));

        if (schema is JsonObject root)
        {
            root["type"] = "object";

            root.Remove("$schema");
            root.Remove("$id");
        }

        var settings = new GeminiPromptExecutionSettings
        {
            ResponseMimeType = "application/json",
            ResponseSchema = schema,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        try
        {
            var result = await chatCompletionService.GetChatMessageContentAsync(
                history,
                kernel: kernel,
                executionSettings: settings);
            var reply = result.Content ?? "Вибач, сталася помилка.";
            return reply;
        }
        catch (HttpOperationException ex)
        {
            Console.WriteLine(ex.ResponseContent);
            throw;
        }
    }
}