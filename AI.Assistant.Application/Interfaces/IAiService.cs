using AI.Assistant.Core.Models;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Application.Interfaces;

public interface IAiService
{
    Task<string> GetAiResponse(ChatHistory history, long chatId, MessageSource source);
}