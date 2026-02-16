namespace AI.Assistant.Core;

public class Settings
{
    public int HistoryMaxLimit { get; set; }
    public int HistoryMinLimit { get; set; }
    public string TavilyApiKey { get; set; } = string.Empty;
}