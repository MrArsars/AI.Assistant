using AI.Assistant.Core.Models;

namespace AI.Assistant.Application.Interfaces;

public interface ISanitizerAgent
{
    Task<UnifiedMessage> SanitizeAsync(string rawText, string inputType, CancellationToken ct = default);
}