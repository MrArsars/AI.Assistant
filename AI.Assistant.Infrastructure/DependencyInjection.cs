using AI.Assistant.Application.Interfaces;
using AI.Assistant.Core.Interfaces;
using AI.Assistant.Infrastructure.Agents;
using AI.Assistant.Infrastructure.Persistence.Mapping;
using AI.Assistant.Infrastructure.Repositories;
using AI.Assistant.Infrastructure.Resilience;
using AI.Assistant.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        services.AddSingleton(new GeminiPromptExecutionSettings()
        {
            ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions
        });

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

        services.AddHttpClient<IVoiceTranscriptionService, VoiceTranscriptionService>(client =>
        {
            client.BaseAddress = new Uri("https://api.assemblyai.com/v2/");
            var assemblyApiKey = config.GetValue<string>("AssemblyAiApiKey");
            client.DefaultRequestHeaders.Add("authorization", assemblyApiKey);
        });

        services.AddHttpClient<IWebService, WebService>();

        services.AddSingleton<IAiService, AiService>();
        services.AddSingleton<ISanitizerAgent, SanitizerAgent>();

        services.AddScoped<IEmbeddingService, EmbeddingService>();

        services.AddTransient<IChatCompletionService>(sp =>
            sp.GetRequiredService<Kernel>().GetRequiredService<IChatCompletionService>());

        var autoMapperLicenseKey = config.GetValue<string>("AutoMapperLicenseKey");
        services.AddAutoMapper(cfg => { cfg.LicenseKey = autoMapperLicenseKey; }, typeof(MappingProfile).Assembly);

        return services;
    }
}