using System.ComponentModel;
using AI.Assistant.Application.Interfaces;
using Microsoft.SemanticKernel;

namespace AI.Assistant.Presentation.Plugins.Plugins;

public class WebSearchPlugin(IWebService webService)
{
    [KernelFunction("search_internet")]
    [Description("Searches the web for information.")]
    public async Task<string> SearchAsync([Description("The search query")] string query)
    {
        var results = await webService.SearchWeb(query);
        return results;
    }
}