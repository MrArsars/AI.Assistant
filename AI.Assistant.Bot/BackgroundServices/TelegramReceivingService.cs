using AI.Assistant.Bot.Handlers;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace AI.Assistant.Bot.BackgroundServices;

public class TelegramReceivingService(BotHandler botHandler, ITelegramBotClient telegramBotClient) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions() { AllowedUpdates = [] };
        telegramBotClient.StartReceiving(
            updateHandler: botHandler.HandleMessageAsync,
            errorHandler: botHandler.HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}