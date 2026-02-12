using System.Text;

namespace AI.Assistant.Core;

public class Settings
{
    public string SystemPrompt { get; set; } = LoadSystemInstruction();
    public int HistoryMaxLimit { get; set; }
    public int HistoryMinLimit { get; set; }
    public string TavilyApiKey { get; set; } = string.Empty;
    
    private static string LoadSystemInstruction()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Prompts", "system_instruction.txt");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Промпт не знайдено за шляхом: {filePath}");

        return File.ReadAllText(filePath, Encoding.UTF8);
    }
}