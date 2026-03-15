namespace AI.Assistant.Application.Interfaces;

public interface IVoiceTranscriptionService
{
    Task<string> TranscriptVoiceMessage(string filePath, CancellationToken cancellationToken);
}