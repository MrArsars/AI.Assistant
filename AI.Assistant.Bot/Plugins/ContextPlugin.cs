using System.ComponentModel;
using AI.Assistant.Bot.Services.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Bot.Plugins;

public class ContextPlugin(IContextService contextService)
{
    [KernelFunction("save_user_fact")]
    [Description("Saves informatoion requested to remember by the user.")]
    public async Task SaveAsync(
        [Description("requested information to save")] string info,
        Kernel kernel)
    {
        var chatId = (long)kernel.Data["chatId"];
        await contextService.SaveContextAsync(chatId, info);
        var history = (ChatHistory)kernel.Data["history"];
        history.AddSystemMessage(info);
    }
}