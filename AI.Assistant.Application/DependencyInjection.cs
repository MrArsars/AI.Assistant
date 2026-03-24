using AI.Assistant.Application.Interfaces;
using AI.Assistant.Application.Publishers;
using AI.Assistant.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Assistant.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IReminderService, ReminderService>();
        services.AddSingleton<ProactivePublisher>();

        services.AddHostedService(provider =>
            provider.GetRequiredService<ProactivePublisher>());

        return services;
    }
}