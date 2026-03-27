using AI.Assistant.Application.Interfaces;
using AI.Assistant.Core.Interfaces;
using AI.Assistant.Infrastructure.Agents;
using AI.Assistant.Infrastructure.Persistence.Mapping;
using AI.Assistant.Infrastructure.Repositories;
using AI.Assistant.Infrastructure.Resilience;
using AI.Assistant.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Polly.Registry;
using Supabase;

namespace AI.Assistant.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<InfrastructureSettings>()
            .Bind(config.GetSection(InfrastructureSettings.SectionName))
            .ValidateOnStart();

        services.AddSingleton(new GeminiPromptExecutionSettings()
        {
            ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions
        });

        var registry = new PolicyRegistry { { "DbRetryPolicy", ResiliencePolicyFactory.GetDbRetryPolicy() } };

        services.AddSingleton<IPolicyRegistry<string>>(registry);

        services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<InfrastructureSettings>>().Value;
            var client = new Client(settings.SupabaseUrl, settings.SupabaseApiToken,
                new SupabaseOptions { AutoConnectRealtime = true });
            client.InitializeAsync().Wait();
            return client;
        });


        services.AddTransient<IMessagesRepository, MessagesRepository>();
        services.AddTransient<IContextManager, ContextRepository>();
        services.AddTransient<IContextProvider, ContextRepository>();
        services.AddTransient<IRemindersRepository, RemindersRepository>();

        services.AddHttpClient<IVoiceTranscriptionService, VoiceTranscriptionService>((sp, client) =>
        {
            var settings = sp.GetRequiredService<IOptions<InfrastructureSettings>>().Value;
            client.BaseAddress = new Uri(settings.AssemblyAiApiUrl);
            client.DefaultRequestHeaders.Add("authorization", settings.AssemblyAiApiKey);
        });

        services.AddHttpClient<IWebService, WebService>();

        services.AddSingleton<IAiService, AiService>();
        services.AddSingleton<ISanitizerAgent, SanitizerAgent>();

        services.AddScoped<IEmbeddingService, EmbeddingService>();

        services.AddTransient<IChatCompletionService>(sp =>
            sp.GetRequiredService<Kernel>().GetRequiredService<IChatCompletionService>());

        services.AddAutoMapper((sp, cfg) =>
        {
            var settings = sp.GetRequiredService<IOptions<InfrastructureSettings>>().Value;
            cfg.LicenseKey = settings.AutoMapperLicenseKey;
        }, typeof(MappingProfile).Assembly);

        return services;
    }
}