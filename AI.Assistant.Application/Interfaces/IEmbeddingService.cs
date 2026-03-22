namespace AI.Assistant.Application.Interfaces;

public interface IEmbeddingService
{
    Task<float[]> GetEmbeddingFromTextAsync(string content, CancellationToken ct);
}