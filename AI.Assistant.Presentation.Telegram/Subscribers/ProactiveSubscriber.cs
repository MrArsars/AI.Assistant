using AI.Assistant.Application.Handlers;
using AI.Assistant.Application.Publishers;
using AI.Assistant.Core.Models;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace AI.Assistant.Presentation.Telegram.Subscribers;

public class ProactiveSubscriber(
    ITelegramBotClient telegramBotClient,
    ProactivePublisher proactivePublisher,
    MessageHandler messageHandler) : IHostedService
{
    public Task StartAsync(CancellationToken ct)
    {
        proactivePublisher.OnMessageReceived += HandleProactive;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken ct)
    {
        proactivePublisher.OnMessageReceived -= HandleProactive;
        return Task.CompletedTask;
    }

    private async Task HandleProactive(object? sender, ReminderEventArgs eventArgs)
    {
        var (chatId, message, source) = eventArgs;

        if (source is not MessageSource.Telegram) return;

        await telegramBotClient.SendMessage(chatId, message);
        await messageHandler.AddProactiveToHistoryAsync(chatId, message);
    }
}