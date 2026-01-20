using System.ComponentModel;
using AI.Assistant.Bot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace AI.Assistant.Bot.Plugins;

public class PermanentMemoryPlugin()
{
    [KernelFunction("save_user_fact")]
    [Description("Saves informatoion requested to remember by the user.")]
    public async Task SaveAsync(
        [Description("requested information to save")] string info,
        Kernel kernel)
    {
        //TODO: review service calling
        var serviceProvider = (IServiceProvider)kernel.Data["sp"];
        using var scope = serviceProvider.CreateScope();
        var chatService = scope.ServiceProvider.GetRequiredService<IChatService>();
        await chatService.SavePermanentAsync((long)kernel.Data["chatId"], info);
    }
}