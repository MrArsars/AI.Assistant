using AI.Assistant.Core.Interfaces;
using AI.Assistant.Infrastructure.Plugins;
using AI.Assistant.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Supabase;

namespace AI.Assistant.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        
        services.AddSingleton(_ => 
        {
            var client = new Client(config["SupabaseUrl"]!, config["SupabaseApiToken"],
                new SupabaseOptions { AutoConnectRealtime = true });
            client.InitializeAsync().Wait();
            return client;
        });
        
        services.AddTransient<IMessagesRepository, MessagesRepository>();
        services.AddTransient<IContextRepository, ContextRepository>();
        services.AddTransient<IRemindersRepository, RemindersRepository>();
        
        services.AddTransient<ContextPlugin>();
        services.AddTransient<WebSearchPlugin>();
        services.AddTransient<RemindersPlugin>();
        
        services.AddSingleton(new GeminiPromptExecutionSettings()
        {
            ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions
        });

        services.AddTransient<Kernel>(sp =>
        {
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.AddGoogleAIGeminiChatCompletion(config["GeminiModel"]!, config["GeminiApiToken"]!);

            kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<ContextPlugin>(), "Context");
            kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<WebSearchPlugin>(), "WebSearch");
            kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<RemindersPlugin>(), "Reminders");

            return kernelBuilder.Build();
        });

        services.AddTransient<IChatCompletionService>(sp =>
            sp.GetRequiredService<Kernel>().GetRequiredService<IChatCompletionService>());
        
        return services;
    }    
}