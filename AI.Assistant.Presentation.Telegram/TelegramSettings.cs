using System.ComponentModel.DataAnnotations;

namespace AI.Assistant.Presentation.Telegram;

public class TelegramSettings
{
    public const string SectionName = "Telegram";
    [Required] public required string TelegramBotToken { get; set; }
}