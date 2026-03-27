using System.Text;
using AI.Assistant.Application;
using AI.Assistant.Application.Handlers;
using AI.Assistant.Core;
using AI.Assistant.Infrastructure;
using AI.Assistant.Presentation.Plugins;
using AI.Assistant.Presentation.Telegram;
using AI.Assistant.Presentation.Telegram.BackgroundServices;
using AI.Assistant.Presentation.Telegram.Handlers;
using AI.Assistant.Presentation.Telegram.Interfaces;
using AI.Assistant.Presentation.Telegram.Services;
using AI.Assistant.Presentation.Telegram.Subscribers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

var config = builder.Configuration;
config.AddUserSecrets<Program>();

builder.Services.AddOptions<TelegramSettings>()
    .Bind(config.GetSection(TelegramSettings.SectionName))
    .ValidateOnStart();

builder.Services.AddCoreServices();
builder.Services.AddInfrastructure(config);
builder.Services.AddApplicationServices(config);

builder.Services.AddPlugins(config);

builder.Services.AddScoped<HttpClient>();
builder.Services.AddTransient<MessageHandler>();

builder.Services.AddSingleton<ITelegramBotClient>(sp =>
    {
        var settings = sp.GetRequiredService<IOptions<TelegramSettings>>().Value;
        return new TelegramBotClient(settings.TelegramBotToken);
    }
);

builder.Services.AddSingleton<BotHandler>();
builder.Services.AddTransient<IFileService, FileService>();

builder.Services.AddHostedService<ProactiveSubscriber>();
builder.Services.AddHostedService<TelegramReceivingService>();

using var host = builder.Build();


Console.WriteLine($"{DateTime.Now:HH:mm:ss} | Бот запущений!");
await host.RunAsync();