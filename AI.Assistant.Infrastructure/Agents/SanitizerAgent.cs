using System.Text.Json;
using AI.Assistant.Application.Interfaces;
using AI.Assistant.Core.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;

namespace AI.Assistant.Infrastructure.Agents;

public class SanitizerAgent(Kernel kernel) : ISanitizerAgent
{
    public async Task<UnifiedMessage> SanitizeAsync(string rawText, string inputType, CancellationToken ct = default)
    {
        var schema = KernelJsonSchema.Parse("""
                                            {
                                              "type": "object",
                                              "properties": {
                                                "content": { "type": "string", "description": "Очищений текст" },
                                                "message_type": { "type": "string", "enum": ["Text", "Voice"] },
                                                "timestamp": { "type": "string" }
                                              },
                                              "required": ["content", "message_type", "timestamp"]
                                            }
                                            """);

        var settings = new GeminiPromptExecutionSettings
        {
            ResponseMimeType = "application/json",
            ResponseSchema = schema,
            ToolCallBehavior = null
        };

        var prompt = $"""
                      Системна інструкція: Ти лінгвістичний коректор. Очисти текст лише від помилок. Якщо таких немає, поверни такий самий текст.
                      Вхідний текст: {rawText}
                      Тип входу: {inputType}
                      Поточний час: {DateTime.UtcNow:ISO 8601}
                      """;

        var result = await kernel.InvokePromptAsync(prompt, new KernelArguments(settings), cancellationToken: ct);

        return JsonSerializer.Deserialize<UnifiedMessage>(result.ToString(),
                   new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower })
               ?? throw new Exception("Failed to sanitize message.");
    }
}