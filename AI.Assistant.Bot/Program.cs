using System.Text;
using AI.Assistant.Bot;
using AI.Assistant.Bot.Handlers;
using AI.Assistant.Bot.Plugins;
using AI.Assistant.Bot.Repositories;
using AI.Assistant.Bot.Repositories.Interfaces;
using AI.Assistant.Bot.Services;
using AI.Assistant.Bot.Services.BackgroundServices;
using AI.Assistant.Bot.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Supabase;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.ChatCompletion;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

var config = builder.Configuration;

var settings = new Settings 
{ 
    HistoryMaxLimit = config.GetValue<int>("HistoryMessagesMaxLimit"),
    HistoryMinLimit = config.GetValue<int>("HistoryMessagesMinLimit"),
    TavilyApiKey = config.GetValue<string>("TavilyApiKey"),
    SystemPrompt = ChatService.LoadSystemInstruction() 
};
builder.Services.AddSingleton(settings);

builder.Services.AddSingleton(_ => 
{
    var client = new Client(config["SupabaseUrl"]!, config["SupabaseApiToken"],
        new SupabaseOptions { AutoConnectRealtime = true });
    client.InitializeAsync().Wait();
    return client;
});

builder.Services.AddSingleton<ITelegramBotClient>(
    new TelegramBotClient(config["TelegramBotToken"]!));


builder.Services.AddTransient<IMessagesRepository, MessagesRepository>();
builder.Services.AddTransient<IContextRepository, ContextRepository>();
builder.Services.AddTransient<IRemindersRepository, RemindersRepository>();

builder.Services.AddTransient<IMessagesService, MessagesService>();
builder.Services.AddTransient<IContextService, ContextService>();
builder.Services.AddTransient<IReminderService, ReminderService>();
builder.Services.AddTransient<IChatService, ChatService>();
builder.Services.AddTransient<ITelegramService, TelegramService>();

builder.Services.AddTransient<ContextPlugin>();
builder.Services.AddTransient<WebSearchPlugin>();
builder.Services.AddTransient<RemindersPlugin>();

builder.Services.AddSingleton<IHistoryService, HistoryService>();

builder.Services.AddSingleton(new GeminiPromptExecutionSettings()
{
    ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions
});

builder.Services.AddTransient<Kernel>(sp =>
{
    var kernelBuilder = Kernel.CreateBuilder();
    kernelBuilder.AddGoogleAIGeminiChatCompletion(config["GeminiModel"]!, config["GeminiApiToken"]!);

    kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<ContextPlugin>(), "Context");
    kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<WebSearchPlugin>(), "WebSearch");
    kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<RemindersPlugin>(), "Reminders");

    return kernelBuilder.Build();
});

builder.Services.AddTransient<IChatCompletionService>(sp =>
    sp.GetRequiredService<Kernel>().GetRequiredService<IChatCompletionService>());

builder.Services.AddSingleton<BotHandler>();
builder.Services.AddHostedService<ProactiveReminderService>();
builder.Services.AddHostedService<TelegramReceivingService>();

using var host = builder.Build();

Console.WriteLine($"{DateTime.Now:HH:mm:ss} | Бот запущений!");
await host.RunAsync();