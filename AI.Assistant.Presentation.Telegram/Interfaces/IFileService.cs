using Telegram.Bot;

namespace AI.Assistant.Presentation.Telegram.Interfaces;

public interface IFileService
{
    Task<Stream?> GetFileStreamAsync(string voiceId, ITelegramBotClient botClient, CancellationToken ct);
}