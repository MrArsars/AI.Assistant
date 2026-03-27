using AI.Assistant.Presentation.Plugins.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace AI.Assistant.Presentation.Plugins;

public static class DependencyInjection
{
    public static IServiceCollection AddPlugins(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<PluginsSettings>()
            .Bind(config.GetSection(PluginsSettings.SectionName))
            .ValidateOnStart();

        services.AddTransient<ContextPlugin>();
        services.AddTransient<WebSearchPlugin>();
        services.AddTransient<RemindersPlugin>();
        services.AddTransient<DateTimePlugin>();


        services.AddTransient<Kernel>(sp =>
        {
            var kernelBuilder = Kernel.CreateBuilder();
            var settings = sp.GetRequiredService<IOptions<PluginsSettings>>().Value;

            kernelBuilder.AddGoogleAIGeminiChatCompletion(settings.GeminiModel, settings.GeminiApiToken);
            kernelBuilder.AddGoogleAIEmbeddingGenerator(settings.EmbeddingModel, settings.GeminiApiToken);

            kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<ContextPlugin>(), "Context");
            kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<DateTimePlugin>(), "DateTime");
            kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<WebSearchPlugin>(), "WebSearch");
            kernelBuilder.Plugins.AddFromObject(sp.GetRequiredService<RemindersPlugin>(), "Reminders");

            return kernelBuilder.Build();
        });

        return services;
    }
}