namespace AI.Assistant.Bot;

public class Settings
{
    public string SystemPrompt { get; set; } = string.Empty;
    public int HistoryLimit { get; set; }
    public string TavilyApiKey { get; set; } = string.Empty;
}