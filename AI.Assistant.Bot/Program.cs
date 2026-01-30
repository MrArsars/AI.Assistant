using System.Text;
using AI.Assistant.Bot;
using AI.Assistant.Bot.Handlers;
using AI.Assistant.Bot.Plugins;
using AI.Assistant.Bot.Repositories;
using AI.Assistant.Bot.Repositories.Interfaces;
using AI.Assistant.Bot.Services;
using AI.Assistant.Bot.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Supabase;
using Telegram.Bot;
using Telegram.Bot.Polling;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

var supabase = new Client(config["SupabaseUrl"]!, config["SupabaseApiToken"],
    new SupabaseOptions { AutoConnectRealtime = true });
await supabase.InitializeAsync();

var systemPrompt = ChatService.LoadSystemInstruction();
var historyMaxLimit = config.GetValue<int>("HistoryMessagesMaxLimit");
var historyMinLimit = config.GetValue<int>("HistoryMessagesMinLimit");
var tavilyApiKey = config["TavilyApiKey"];


var settings = new Settings() { HistoryMinLimit = historyMinLimit, HistoryMaxLimit = historyMaxLimit, SystemPrompt = systemPrompt, TavilyApiKey = tavilyApiKey };

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton(settings);
builder.Services.AddSingleton<Supabase.Client>(supabase);
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(config["TelegramBotToken"]!));
builder.Services.AddSingleton<IChatService, ChatService>();
builder.Services.AddSingleton<IMessagesRepository, MessagesRepository>();
builder.Services.AddSingleton<IContextRepository, ContextRepository>();
builder.Services.AddSingleton<IRemindersRepository, RemindersRepository>();
builder.Services.AddSingleton<IHistoryService, HistoryService>();
builder.Services.AddSingleton<IContextService, ContextService>();
builder.Services.AddSingleton<IReminderService, ReminderService>();
builder.Services.AddSingleton<ContextPlugin>();
builder.Services.AddSingleton<WebSearchPlugin>();
builder.Services.AddSingleton<RemindersPlugin>();
builder.Services.AddSingleton<BotHandler>();

builder.Services.AddHostedService<ProactiveReminderService>();

builder.Services.AddSingleton(new GeminiPromptExecutionSettings()
{
    ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions
});
builder.Services.AddSingleton<Kernel>(sp =>
{
    var kernelBuilder = Kernel.CreateBuilder();

    kernelBuilder.AddGoogleAIGeminiChatCompletion(config["GeminiModel"]!, config["GeminiApiToken"]!);

    var contextPlugin = sp.GetRequiredService<ContextPlugin>();
    var webSearchPlugin = sp.GetRequiredService<WebSearchPlugin>();
    var reminderPlugin = sp.GetRequiredService<RemindersPlugin>();
    kernelBuilder.Plugins.AddFromObject(contextPlugin, "Context");
    kernelBuilder.Plugins.AddFromObject(webSearchPlugin, "WebSearch");
    kernelBuilder.Plugins.AddFromObject(reminderPlugin, "Reminders");

    return kernelBuilder.Build();
});
builder.Services.AddSingleton<IChatCompletionService>(sp =>
    sp.GetRequiredService<Kernel>().GetRequiredService<IChatCompletionService>());

using var host = builder.Build();

var botHandler = host.Services.GetRequiredService<BotHandler>();
var botClient = host.Services.GetRequiredService<ITelegramBotClient>();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = []
};

botClient.StartReceiving(
    updateHandler: botHandler.HandleUpdateAsync,
    errorHandler: botHandler.HandlePollingErrorAsync,
    receiverOptions: receiverOptions
);

Console.WriteLine("Бот запущений та готовий до роботи!");
await host.RunAsync();