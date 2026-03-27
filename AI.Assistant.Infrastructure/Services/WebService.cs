using System.Text;
using System.Text.Json;
using AI.Assistant.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace AI.Assistant.Infrastructure.Services;

public class WebService(HttpClient httpClient, IOptions<InfrastructureSettings> settings) : IWebService
{
    public async Task<string> SearchWeb(string query)
    {
        var request = new
        {
            api_key = settings.Value.TavilyApiKey,
            query,
            search_depth = "basic",
            max_results = 3
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(settings.Value.TavilySearchUrl, content);

        if (!response.IsSuccessStatusCode) return "Пошук тимчасово недоступний.";

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var results = doc.RootElement.GetProperty("results").EnumerateArray()
            .Select(r => $"Source: {r.GetProperty("title")}\nContent: {r.GetProperty("content")}");

        return string.Join("\n\n", results);
    }
}