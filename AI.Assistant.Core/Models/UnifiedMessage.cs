namespace AI.Assistant.Core.Models;

public record UnifiedMessage(
    string Content,
    string MessageType,
    string Timestamp,
    double? Confidence = null
);