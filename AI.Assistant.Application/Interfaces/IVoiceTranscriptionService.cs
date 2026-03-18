namespace AI.Assistant.Application.Interfaces;

public interface IVoiceTranscriptionService
{
    Task<string> TranscriptVoiceMessage(Stream memoryStream, CancellationToken cancellationToken);
}