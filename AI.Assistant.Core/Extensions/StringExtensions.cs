using AI.Assistant.Core.Models;

namespace AI.Assistant.Core.Extensions;

public static class StringExtensions
{
    public static IEnumerable<string> ChunkBy(this string text, int chunkSize = 4000)
    {
        if (string.IsNullOrEmpty(text)) yield break;

        for (var i = 0; i < text.Length; i += chunkSize)
        {
            yield return text.Substring(i, Math.Min(chunkSize, text.Length - i));
        }
    }
    
    public static MessageSource ToMessageSource(this string? src)
    {
        return src switch
        {
            "Telegram" => MessageSource.Telegram,
            "MobileApp" => MessageSource.MobileApp,
            _ => throw new Exception("unhandled message source")
        };
    }
}