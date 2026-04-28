using System.ComponentModel;
using System.Text.Json.Serialization;

namespace AI.Assistant.Core.Models;

public class AiRequest
{
    [Description("Вхідне повідомлення користувача.")]
    [JsonPropertyName("content")]
    public string Content { get; set; }

    [Description("Тип вхідного повідомлення для контексту.")]
    [JsonPropertyName("message_type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MessageType MessageType { get; set; }

    [Description("Платформа з якої надійшло повідомлення.")]
    [JsonPropertyName("message_source")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MessageSource MessageSource { get; set; }

    [Description("Час відправки повідомлення.")]
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }

    [JsonConstructor]
    public AiRequest()
    {
    }

    public AiRequest(string message, MessageType type, MessageSource source)
    {
        Content = message;
        MessageType = type;
        MessageSource = source;
        Timestamp = DateTimeOffset.UtcNow.ToString();
    }
}