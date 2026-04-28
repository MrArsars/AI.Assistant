using System.ComponentModel;
using System.Text.Json.Serialization;

namespace AI.Assistant.Core.Models;

public record AiResponse
{
    [Description("Очищений від помилок текст, адаптований під стиль користувача.")]
    [JsonPropertyName("content")]
    public required string Content { get; set; }

    [Description("Тип повідомлення.")]
    [JsonPropertyName("response_type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ResponseType ResponseType { get; set; }

    [Description("Час відправки повідомлення.")]
    [JsonPropertyName("timestamp")]
    public required string Timestamp { get; set; }
}