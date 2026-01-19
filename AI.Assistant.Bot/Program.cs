using System.Text;
using AI.Assistant.Bot.Handlers;
using AI.Assistant.Bot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Supabase;
using Telegram.Bot;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

var supabase = new Supabase.Client(config["SupabaseUrl"]!, config["SupabaseApiToken"],
    new SupabaseOptions { AutoConnectRealtime = true });
await supabase.InitializeAsync();

var kernel = Kernel.CreateBuilder()
    .AddGoogleAIGeminiChatCompletion(config["GeminiModel"]!, config["GeminiApiToken"]!)
    .Build();

var chatService = new ChatService(supabase, config.GetValue<int>("HistoryMessagesLimit"));
var chatCompletion = kernel.GetRequiredService<IChatCompletionService>();
var botClient = new TelegramBotClient(config["TelegramBotToken"]!);
var systemPrompt = ChatService.LoadSystemInstruction();

var botHandler = new BotHandler(
    botClient,
    chatCompletion,
    chatService,
    kernel,
    systemPrompt);

botClient.OnMessage += botHandler.HandleUpdateAsync;

Console.WriteLine("Бот запущений та готовий до роботи!");
await Task.Delay(-1);