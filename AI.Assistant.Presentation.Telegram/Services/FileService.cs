using AI.Assistant.Presentation.Telegram.Interfaces;
using Telegram.Bot;

namespace AI.Assistant.Presentation.Telegram.Services;

public class FileService : IFileService
{
    public async Task<Stream?> GetFileStreamAsync(string fileId, ITelegramBotClient botClient, CancellationToken ct)
    {
        var file = await botClient.GetFile(fileId, ct);
        if (file.FilePath == null) return null;

        var ms = new MemoryStream();
        await botClient.DownloadFile(file.FilePath, ms, ct);
        ms.Position = 0;

        return ms;
    }
}