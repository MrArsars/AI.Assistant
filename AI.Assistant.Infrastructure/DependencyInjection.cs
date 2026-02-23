using AI.Assistant.Application.Interfaces;
using AI.Assistant.Core.Interfaces;
using AI.Assistant.Infrastructure.Repositories;
using AI.Assistant.Infrastructure.Resilience;
using AI.Assistant.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Polly.Registry;
using Supabase;

namespace AI.Assistant.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var registry = new PolicyRegistry { { "DbRetryPolicy", ResiliencePolicyFactory.GetDbRetryPolicy() } };

        services.AddSingleton<IPolicyRegistry<string>>(registry);

        services.AddSingleton(_ =>
        {
            var client = new Client(config["SupabaseUrl"]!, config["SupabaseApiToken"],
                new SupabaseOptions { AutoConnectRealtime = true });
            client.InitializeAsync().Wait();
            return client;
        });

        services.AddTransient<IMessagesRepository, MessagesRepository>();
        services.AddTransient<IContextManager, ContextRepository>();
        services.AddTransient<IContextProvider, ContextRepository>();
        services.AddTransient<IRemindersRepository, RemindersRepository>();

        services.AddSingleton<IAiService, AiService>();

        services.AddTransient<IChatCompletionService>(sp =>
            sp.GetRequiredService<Kernel>().GetRequiredService<IChatCompletionService>());

        return services;
    }
}