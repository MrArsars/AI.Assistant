using System.Text;
using AI.Assistant.Bot.Handlers;
using AI.Assistant.Bot.Plugins;
using AI.Assistant.Bot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Supabase;
using Telegram.Bot;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

var supabase = new Client(config["SupabaseUrl"]!, config["SupabaseApiToken"],
    new SupabaseOptions { AutoConnectRealtime = true });
await supabase.InitializeAsync();

var builder = Kernel.CreateBuilder()
    .AddGoogleAIGeminiChatCompletion(config["GeminiModel"]!, config["GeminiApiToken"]!);

var botClient = new TelegramBotClient(config["TelegramBotToken"]!);
var systemPrompt = ChatService.LoadSystemInstruction();

var serviceCollection = new ServiceCollection();
serviceCollection.AddScoped<IChatService>(sp =>
{
    var historyLimit = config.GetValue<int>("HistoryMessagesLimit");
    return new ChatService(supabase, historyLimit);
});
var serviceProvider = serviceCollection.BuildServiceProvider();

var chatService = serviceProvider.GetRequiredService<IChatService>();

builder.Plugins.AddFromType<PermanentMemoryPlugin>("Memory");
var kernel = builder.Build();
var chatCompletion = kernel.GetRequiredService<IChatCompletionService>();
kernel.Data["sp"] = serviceProvider;

var botHandler = new BotHandler(
    botClient,
    chatCompletion,
    kernel,
    chatService,
    systemPrompt);

botClient.OnMessage += botHandler.HandleUpdateAsync;

Console.WriteLine("Бот запущений та готовий до роботи!");
await Task.Delay(-1);