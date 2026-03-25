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
        };
        services.AddSingleton(settings);

        return services;
    }
}