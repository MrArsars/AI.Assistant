using AI.Assistant.Application.Interfaces;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;

namespace AI.Assistant.Infrastructure.Services;

public class EmbeddingService(Kernel kernel) : IEmbeddingService
{
    public async Task<float[]> GetEmbeddingFromTextAsync(string content, CancellationToken ct)
    {
        var embeddingGenerator = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

        var embedding = await embeddingGenerator.GenerateAsync([content], null, ct);
        var vectorArray = embedding[0].Vector.ToArray();

        return vectorArray;
    }
}