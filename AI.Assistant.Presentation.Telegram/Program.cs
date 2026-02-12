using System.Text;
using AI.Assistant.Application;
using AI.Assistant.Application.Handlers;
using AI.Assistant.Presentation.Bot.BackgroundServices;
using AI.Assistant.Presentation.Bot.Handlers;
using AI.Assistant.Core;
using AI.Assistant.Infrastructure;
using AI.Assistant.Presentation.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

var config = builder.Configuration;
config.AddUserSecrets<Program>();

builder.Services.AddCoreServices(config);
builder.Services.AddInfrastructure(config);
builder.Services.AddApplicationServices();
builder.Services.AddPlugins(config);
builder.Services.AddTransient<MessageHandler>();

builder.Services.AddSingleton<ITelegramBotClient>(
    new TelegramBotClient(config["TelegramBotToken"]!));

builder.Services.AddSingleton<BotHandler>();
builder.Services.AddHostedService<ProactiveReminderService>();
builder.Services.AddHostedService<TelegramReceivingService>();

using var host = builder.Build();

Console.WriteLine($"{DateTime.Now:HH:mm:ss} | Бот запущений!");
await host.RunAsync();