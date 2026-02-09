using AI.Assistant.Core.Interfaces;
using AI.Assistant.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Assistant.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration config)
    {
        var settings = new Settings 
        { 
            HistoryMaxLimit = config.GetValue<int>("HistoryMessagesMaxLimit"),
            HistoryMinLimit = config.GetValue<int>("HistoryMessagesMinLimit"),
            TavilyApiKey = config.GetValue<string>("TavilyApiKey"),
            SystemPrompt = ChatService.LoadSystemInstruction() 
        };
        services.AddSingleton(settings);
        
        services.AddTransient<IMessagesService, MessagesService>();
        services.AddTransient<IContextService, ContextService>();
        services.AddTransient<IReminderService, ReminderService>();
        services.AddTransient<IChatService, ChatService>();
        services.AddTransient<ITelegramService, TelegramService>();
        services.AddSingleton<IHistoryService, HistoryService>();
        
        return services;
    }
}