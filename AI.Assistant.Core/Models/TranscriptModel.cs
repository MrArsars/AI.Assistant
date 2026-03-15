using System.Text.Json.Serialization;

namespace AI.Assistant.Core.Models;

public class TranscriptModel
{
    public string Id { get; set; }
    public string Status { get; set; }
    public string Text { get; set; }

    [JsonPropertyName("language_code")] public string LanguageCode { get; set; }
    public string Error { get; set; }
}