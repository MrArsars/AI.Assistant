using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AI.Assistant.Core.Models;

[Table("Messages")]
public class MessageModel : BaseModel
{
    [PrimaryKey("id", false)] public Guid? Id { get; set; }
    [Column("chat_id")] public long ChatId { get; set; }
    [Column("role")] public string Role { get; set; } = string.Empty;
    [Column("text")] public string Text { get; set; } = string.Empty;
    [Column("created_at")] public DateTime CreatedAt { get; set; }

    [Column("embedding")]
    [JsonConverter(typeof(VectorConverter))]
    public float[]? Embedding { get; set; }

    public MessageModel()
    {
    }

    public MessageModel(long chatId, string role, string text, float[]? embedding)
    {
        ChatId = chatId;
        Role = role;
        Text = text;
        CreatedAt = DateTime.Now;
        Embedding = embedding;
    }

    private class VectorConverter : JsonConverter<float[]>
    {
        public override void WriteJson(JsonWriter writer, float[] value, JsonSerializer serializer)
        {
            writer.WriteValue($"[{string.Join(",", value)}]");
        }

        public override float[] ReadJson(JsonReader reader, Type objectType, float[] existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            var s = reader.Value?.ToString();
            if (string.IsNullOrEmpty(s)) return null;

            return s.Trim('[', ']').Split(',')
                .Select(float.Parse)
                .ToArray();
        }
    }
}