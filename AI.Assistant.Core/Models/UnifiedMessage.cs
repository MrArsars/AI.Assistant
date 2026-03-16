namespace AI.Assistant.Core.Models;

public record UnifiedMessage(
    string Content,
    string MessageType,
    DateTime Timestamp,
    double? Confidence = null
);