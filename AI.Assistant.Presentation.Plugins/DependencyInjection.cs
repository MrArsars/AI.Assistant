using AI.Assistant.Presentation.Plugins.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;

namespace AI.Assistant.Presentation.Plugins;

public static class DependencyInjection
{
    public static IServiceCollection AddPlugins(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton(new GeminiPromptExecutionSettings()
        {
            ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions
        });
        
        services.AddTransient<ContextPlugin>();
        services.AddTransient<WebSearchPlugin>();
        services.AddTransient<RemindersPlugin>();
        
        services.AddTransient<Kernel>(sp =>
        {
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.AddGoogleAIGeminiChatCompletion(config["GeminiModel"]!, config["GeminiApiToken"]!);

            kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<ContextPlugin>(), "Context");
            kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<WebSearchPlugin>(), "WebSearch");
            kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<RemindersPlugin>(), "Reminders");

            return kernelBuilder.Build();
        });

        return services;
    }
}