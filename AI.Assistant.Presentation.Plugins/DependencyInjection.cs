using AI.Assistant.Presentation.Plugins.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace AI.Assistant.Presentation.Plugins;

public static class DependencyInjection
{
    public static IServiceCollection AddPlugins(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<ContextPlugin>();
        services.AddTransient<WebSearchPlugin>();
        services.AddTransient<RemindersPlugin>();
        services.AddTransient<DateTimePlugin>();


        services.AddTransient<Kernel>(sp =>
        {
            var kernelBuilder = Kernel.CreateBuilder();
            var apiKey = config["GeminiApiToken"]!;

            kernelBuilder.AddGoogleAIGeminiChatCompletion(config["GeminiModel"]!, apiKey);
            kernelBuilder.AddGoogleAIEmbeddingGenerator("gemini-embedding-001", apiKey);

            kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<ContextPlugin>(), "Context");
            kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<DateTimePlugin>(), "DateTime");
            kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<WebSearchPlugin>(), "WebSearch");
            kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<RemindersPlugin>(), "Reminders");

            return kernelBuilder.Build();
        });

        return services;
    }
}