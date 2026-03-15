using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AI.Assistant.Application.Interfaces;
using AI.Assistant.Core.Models;

namespace AI.Assistant.Infrastructure.Services;

public class VoiceTranscriptionService(HttpClient httpClient) : IVoiceTranscriptionService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public async Task<string> TranscriptVoiceMessage(string filePath, CancellationToken ct = default)
    {
        var audioUrl = await UploadFile(filePath, ct);
        var transcript = await GenerateTranscript(audioUrl, ct);

        return await PollForTranscriptResult(transcript.Id, ct);
    }

    private async Task<string> UploadFile(string filePath, CancellationToken ct)
    {
        await using var fileStream = File.OpenRead(filePath);
        using var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        using var response = await httpClient.PostAsync("upload", fileContent, ct);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken: ct);
        return result?.RootElement.GetProperty("upload_url").GetString()
               ?? throw new Exception("Failed to get upload URL");
    }

    private async Task<TranscriptModel> GenerateTranscript(string audioUrl, CancellationToken ct)
    {
        var requestBody = new
        {
            audio_url = audioUrl,
            language_detection = false,
            speech_models = new[] { "universal-2" },
            language_code = "uk",
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestBody, JsonOptions),
            Encoding.UTF8,
            "application/json");

        using var response = await httpClient.PostAsync("transcript", content, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            throw new Exception($"AssemblyAI Post Error: {error}");
        }

        return await response.Content.ReadFromJsonAsync<TranscriptModel>(JsonOptions, ct)
               ?? throw new Exception("Failed to deserialize transcript model");
    }

    private async Task<string> PollForTranscriptResult(string transcriptId, CancellationToken ct)
    {
        var endpoint = $"transcript/{transcriptId}";
        var startTime = DateTime.UtcNow;

        while (DateTime.UtcNow - startTime < TimeSpan.FromMinutes(5))
        {
            ct.ThrowIfCancellationRequested();

            var response = await httpClient.GetAsync(endpoint, ct);
            var result = await response.Content.ReadFromJsonAsync<TranscriptModel>(JsonOptions, ct);

            switch (result?.Status)
            {
                case "completed":
                    return result.Text;
                case "error":
                    throw new Exception($"AssemblyAI Polling Error: {result.Error}");
                case "processing":
                case "queued":
                    await Task.Delay(TimeSpan.FromSeconds(3), ct);
                    break;
                default:
                    throw new Exception($"Unknown status: {result?.Status}");
            }
        }

        throw new TimeoutException("Transcription timed out.");
    }
}