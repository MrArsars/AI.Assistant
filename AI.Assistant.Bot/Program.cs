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
var historyLimit = config.GetValue<int>("HistoryMessagesLimit");
var tavilyApiKey = config["TavilyApiKey"];


var settings = new Settings() {HistoryLimit =  historyLimit, SystemPrompt = systemPrompt, TavilyApiKey = tavilyApiKey};

var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton(settings);
serviceCollection.AddSingleton<Supabase.Client>(supabase);
serviceCollection.AddSingleton<ITelegramBotClient>(new TelegramBotClient(config["TelegramBotToken"]!));
serviceCollection.AddSingleton<IChatService, ChatService>();
serviceCollection.AddSingleton<IMessagesRepository, MessagesRepository>();
serviceCollection.AddSingleton<IContextRepository, ContextRepository>();
serviceCollection.AddSingleton<IHistoryService, HistoryService>();
serviceCollection.AddSingleton<IContextService, ContextService>();
serviceCollection.AddSingleton<ContextPlugin>();
serviceCollection.AddSingleton<WebSearchPlugin>();
serviceCollection.AddSingleton<BotHandler>();
serviceCollection.AddSingleton(new GeminiPromptExecutionSettings() 
{ 
    ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions
});
serviceCollection.AddSingleton<Kernel>(sp =>
{
    var kernelBuilder = Kernel.CreateBuilder();

    kernelBuilder.AddGoogleAIGeminiChatCompletion(config["GeminiModel"]!, config["GeminiApiToken"]!);

    var contextPlugin = sp.GetRequiredService<ContextPlugin>();
    var webSearchPlugin = sp.GetRequiredService<WebSearchPlugin>();
    kernelBuilder.Plugins.AddFromObject(contextPlugin, "Context");
    kernelBuilder.Plugins.AddFromObject(webSearchPlugin, "WebSearch");

    return kernelBuilder.Build();
});
serviceCollection.AddSingleton<IChatCompletionService>(sp => 
    sp.GetRequiredService<Kernel>().GetRequiredService<IChatCompletionService>());

var serviceProvider = serviceCollection.BuildServiceProvider();

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