using AI.Assistant.Application.Interfaces;
using AI.Assistant.Application.Publishers;
using AI.Assistant.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Assistant.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<ApplicationSettings>()
            .Bind(config.GetSection(ApplicationSettings.SectionName))
            .ValidateOnStart();

        services.AddTransient<IReminderService, ReminderService>();
        services.AddSingleton<ProactivePublisher>();

        services.AddTransient<IMessagesService, MessagesService>();
        services.AddSingleton<IHistoryService, HistoryService>();

        services.AddHostedService(provider =>
            provider.GetRequiredService<ProactivePublisher>());

        return services;
    }
}