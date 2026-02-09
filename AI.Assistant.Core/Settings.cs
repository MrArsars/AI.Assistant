namespace AI.Assistant.Core;

public class Settings
{
    public string SystemPrompt { get; set; } = string.Empty;
    public int HistoryMaxLimit { get; set; }
    public int HistoryMinLimit { get; set; }
    public string TavilyApiKey { get; set; } = string.Empty;
}