using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Application.Interfaces;

public interface IAiService
{
    Task<string> GetAiResponse(ChatHistory history, long chatId);
}