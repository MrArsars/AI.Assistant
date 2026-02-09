using System.ComponentModel;
using System.Text;
using System.Text.Json;
using AI.Assistant.Core;
using Microsoft.SemanticKernel;

namespace AI.Assistant.Infrastructure.Plugins;

public class WebSearchPlugin(Settings settings)
{
    private readonly HttpClient _httpClient = new();

    //TODO: Refactor
    [KernelFunction("search_internet")]
    [Description("Searches the web for information.")]
    public async Task<string> SearchAsync([Description("The search query")] string query)
    {
        var request = new
        {
            api_key = settings.TavilyApiKey,
            query,
            search_depth = "basic",
            max_results = 3
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://api.tavily.com/search", content);

        if (!response.IsSuccessStatusCode) return "Пошук тимчасово недоступний.";

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var results = doc.RootElement.GetProperty("results").EnumerateArray()
            .Select(r => $"Source: {r.GetProperty("title")}\nContent: {r.GetProperty("content")}");

        return string.Join("\n\n", results);
    }
}