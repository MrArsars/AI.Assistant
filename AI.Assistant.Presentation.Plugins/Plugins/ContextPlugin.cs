using System.ComponentModel;
using AI.Assistant.Application.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AI.Assistant.Presentation.Plugins.Plugins;

public class ContextPlugin(IContextManager contextManager)
{
    [KernelFunction("save_user_fact")]
    [Description("Saves information explicitly requested by the user.")]
    public async Task SaveAsync(string info, Kernel kernel) => await ExecuteAsync(info, kernel);

    [KernelFunction("update_user_profile")]
    [Description("Saves STABLE long-term facts about the user. DO NOT use for temporary requests, current tasks, or chat summaries. " +
                 "Call this tool every time you discover a new personal fact about the user (location, language, hobbies).")]
    public async Task UpdateProfileAsync(
        [Description("Stable fact: e.g. 'Works as a Java Dev', 'Has a cat named Barsik'. Avoid: 'Asked for a roadmap'.")] string info,
        Kernel kernel) => await ExecuteAsync(info, kernel);

    private async Task ExecuteAsync(string info, Kernel kernel)
    {
        if (kernel.Data.TryGetValue("chatId", out var objId) && objId is long chatId &&
            kernel.Data.TryGetValue("history", out var objHistory) && objHistory is ChatHistory history)
        {
            await contextManager.SaveContextAsync(chatId, info);
        }
    }
}