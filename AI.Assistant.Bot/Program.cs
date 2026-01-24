using System.Text;
using AI.Assistant.Bot;
using AI.Assistant.Bot.Handlers;
using AI.Assistant.Bot.Plugins;
using AI.Assistant.Bot.Repositories;
using AI.Assistant.Bot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Supabase;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

var supabase = new Client(config["SupabaseUrl"]!, config["SupabaseApiToken"],
    new SupabaseOptions { AutoConnectRealtime = true });
await supabase.InitializeAsync();

var builder = Kernel.CreateBuilder()
    .AddGoogleAIGeminiChatCompletion(config["GeminiModel"]!, config["GeminiApiToken"]!);

var systemPrompt = ChatService.LoadSystemInstruction();
var historyLimit = config.GetValue<int>("HistoryMessagesLimit");

builder.Plugins.AddFromType<PermanentMemoryPlugin>("Memory");
var kernel = builder.Build();
var chatCompletion = kernel.GetRequiredService<IChatCompletionService>();

var settings = new Settings() {HistoryLimit =  historyLimit, SystemPrompt = systemPrompt};

var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton(settings);
serviceCollection.AddSingleton<Supabase.Client>(supabase);
serviceCollection.AddSingleton<ITelegramBotClient>(new TelegramBotClient(config["TelegramBotToken"]!));
serviceCollection.AddSingleton<IChatService, ChatService>();
serviceCollection.AddSingleton<IMessagesRepository, MessagesRepository>();
serviceCollection.AddSingleton<IHistoryService, HistoryService>();
serviceCollection.AddSingleton<BotHandler>();
serviceCollection.AddSingleton<Kernel>(kernel);
serviceCollection.AddSingleton<IChatCompletionService>(chatCompletion);
serviceCollection.AddSingleton(new GeminiPromptExecutionSettings() 
{ 
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() 
});

var serviceProvider = serviceCollection.BuildServiceProvider();

kernel.Data["sp"] = serviceProvider;

var botHandler = serviceProvider.GetRequiredService<BotHandler>();
var botClient = serviceProvider.GetRequiredService<ITelegramBotClient>();

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
await Task.Delay(-1);